module MusicStore.Form

open System

open Suave.Model

let bindForm key = Binding.form key Choice1Of2

type TextFieldProperty =
    | MinLength of int
    | MaxLength of int

let testText (text : string) = function
    | MinLength len -> text.Length >= len
    | MaxLength len -> text.Length <= len

let reasonText = function
    | MinLength len -> sprintf "must be at least %d characters" len
    | MaxLength len -> sprintf "must be at most %d characters" len

let verifyText value prop name =
    if testText value prop then Choice1Of2 value
    else sprintf "'%s' %s" name (reasonText prop) |> Choice2Of2

type DecimalFieldProperty =
    | Min of decimal
    | Max of decimal

let testDecimal (decimal : decimal) = function
    | Min min -> decimal >= min
    | Max max -> decimal <= max

let reasonDecimal = function
    | Min min -> sprintf "must be at least %M" min
    | Max max -> sprintf "must be at most %M" max

let verifyDecimal value prop name =
    if testDecimal value prop then Choice1Of2 value
    else sprintf "'%s' %s" name (reasonDecimal prop) |> Choice2Of2


type FormFieldName<'a>(name : string) = 
    member __.Name = name
type TextFieldName(name) =
    inherit FormFieldName<string>(name)
type DecimalFieldName(name) =
    inherit FormFieldName<decimal>(name)

type FormField = 
    | TextField of TextFieldName * TextFieldProperty list
    | DecimalField of DecimalFieldName * DecimalFieldProperty list

type FormResult = {
    GetText : TextFieldName -> string
    GetDecimal : DecimalFieldName -> decimal
}

type ServerSideValidation = FormResult -> bool * string

type Form<'a> = 
    | Form of FormField list * ServerSideValidation list

open Suave.Utils

let verify (field, value : obj) = 
    match (field, value) with
    | TextField (name, props), (:? string as value) ->
        props
        |> List.fold
            (fun value prop ->
                value |> Choice.bind (fun value -> verifyText value prop name.Name))
            (Choice1Of2 value)
        |> Choice.map box
    | DecimalField (name, props), (:? decimal as value) ->
        props
        |> List.fold
            (fun value prop ->
                value |> Choice.bind (fun value -> verifyDecimal value prop name.Name))
            (Choice1Of2 value)
        |> Choice.map box
    | _, v -> failwithf "Unexpected type %s" (v.GetType().FullName)

let name = function
    | TextField (name, _) -> name.Name
    | DecimalField (name, _) -> name.Name

let parse = function
    | TextField _, value -> Choice1Of2 value |> Choice.map box
    | DecimalField _, value -> Parse.decimal value |> Choice.map box

let bindingForm form req =
    let (Form (fields, validations)) = form
    let names = fields |> List.map name
    let values =
        names
        |> List.fold 
            (fun ch name -> 
                ch |> Choice.bind (fun xs -> 
                    req |> bindForm name |> Choice.map (fun x -> x :: xs))) 
            (Choice1Of2 [])
        |> Choice.map List.rev 

    values
    |> Choice.bind (fun values ->
        (fields, values)
        ||> List.zip
        |> List.fold
            (fun map (field,value) ->
                map |> Choice.bind (fun map ->
                    parse (field, value) 
                    |> Choice.bind(fun value -> 
                        verify (field,value) 
                        |> Choice.map (fun x -> 
                            map |> Map.add (name field) x))))
            (Choice1Of2 Map.empty)
        |> Choice.map (fun map ->
            let result = {
                GetText = fun key -> map.[key.Name] :?> _
                GetDecimal = fun key -> map.[key.Name] :?> _
            }
            result
        ))
    |> Choice.bind (fun result ->
        validations
        |> List.fold
            (fun result validation -> 
                result 
                |> Choice.bind (fun result -> 
                    match validation result with
                    | true, _ -> Choice1Of2 result
                    | false, reason -> Choice2Of2 reason))
            (Choice1Of2 result))

module Logon = 
    let Username = TextFieldName("username")
    let Password = TextFieldName("password")
    
    let form = 
        Form([ TextField(Username, 
                         [ MinLength 5
                           MaxLength 20 ])
               TextField(Password, []) ], [])
(*
let logonForm req =
    binding {
        let! username = req |> bindForm "usern`ame"
        let! password = req |> bindForm "password"

        return username,password
    }
*)

let logonForm req = bindingForm Logon.form req

module Register =
    let Username = TextFieldName("username")
    let Email = TextFieldName("email")
    let Password = TextFieldName("password")
    let ConfirmPassword = TextFieldName("confirmpassword")

    let passwordsMatch result =
        result.GetText Password = result.GetText ConfirmPassword, "Passwords must match"
    
    let form = 
        Form ([ TextField(Username, [])
                TextField(Email, [])
                TextField(Password, [])
                TextField(ConfirmPassword, []) ], [ passwordsMatch ])

let passHash (pass: string) =
    use sha = Security.Cryptography.SHA256.Create()
    Text.Encoding.UTF8.GetBytes(pass)
    |> sha.ComputeHash
    |> Array.map (fun b -> b.ToString("x2"))
    |> String.concat ""

let registerForm req = bindingForm Register.form req

(*
let registerForm request =
    binding {
        let! username = request |> bindForm "username"
        let! email = request |> bindForm "email"
        let! password = request |> bindForm "password"
        let matchesPassword = function 
        | p when p = password -> Choice1Of2 password 
        | _ -> Choice2Of2 "Passwords do not match"
        let! confirmpassword = request |> bindForm "confirmpassword" >>. matchesPassword

        return (fun (user : Db.User) ->
            user.UserName <- username
            user.Email <- email
            user.Password <- passHash password
        )
    }
*)

let albumForm req = 
    binding {
        let! artistId = req |> bindForm "artist" >>. Parse.int32
        let! genreId = req |> bindForm "genre" >>. Parse.int32
        let! title = req |> bindForm "title"
        let! price = req |> bindForm "price" >>. Parse.decimal
        let! artUrl = req |> bindForm "artUrl"

        return (fun (album : Db.Album) -> 
            album.ArtistId <- artistId
            album.GenreId <- genreId
            album.Title <- title
            album.Price <- price
            album.AlbumArtUrl <- artUrl
        )
    }
