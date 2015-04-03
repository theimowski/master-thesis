module MusicStore.Form

open System

open Suave.Model

let bindForm key = Binding.form key Choice1Of2

let passHash (pass: string) =
    use sha = Security.Cryptography.SHA256.Create()
    Text.Encoding.UTF8.GetBytes(pass)
    |> sha.ComputeHash
    |> Array.map (fun b -> b.ToString("x2"))
    |> String.concat ""

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
    else name + " " + reasonText prop |> Choice2Of2

type FormField = 
    | TextField of string * TextFieldProperty list

open Suave.Utils

let verify = function
    | TextField (name, props), value ->
        props
        |> List.fold
            (fun value prop ->
                value |> Choice.bind (fun value -> verifyText value prop name))
            (Choice1Of2 value)

let name = function
    | TextField (name, _) -> name

type Form = 
    | Form of FormField list

let bindingForm form req =
    let (Form fields) = form
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
                    verify (field,value) |> Choice.map (fun x -> map |> Map.add (name field) x)))
            (Choice1Of2 Map.empty)
        |> Choice.map (fun map ->
            fun key -> map.[key]
        )
    )

module Logon = 
    let Username = "username"
    let Password = "password"
    
    let form = 
        Form [ TextField(Username, 
                         [ MinLength 5
                           MaxLength 20 ])
               TextField(Password, []) ]

let x = ["a", box "b"] |> dict
let get<'a> key = x.[key] :?> 'a
let test : string = get "a"

(*
let logonForm req =
    binding {
        let! username = req |> bindForm "username"
        let! password = req |> bindForm "password"

        return username,password
    }
*)

let logonForm = bindingForm Logon.form