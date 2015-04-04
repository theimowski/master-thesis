module MusicStore.FormUtils

open System

open Suave.Model
open Suave.Utils

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
    | Minimum of decimal
    | Maximum of decimal
    | Step of decimal

let testDecimal (decimal : decimal) = function
    | Minimum min -> decimal >= min
    | Maximum max -> decimal <= max
    | Step step -> decimal % step = 0.0M

let reasonDecimal = function
    | Minimum min -> sprintf "must be at least %M" min
    | Maximum max -> sprintf "must be at most %M" max
    | Step step -> sprintf "must be a multiply of %M" step

let verifyDecimal value prop name =
    if testDecimal value prop then Choice1Of2 value
    else sprintf "'%s' %s" name (reasonDecimal prop) |> Choice2Of2

type IntegerFieldProperty =
    | Min of int
    | Max of int

let testInteger (int : int) = function
    | Min min -> int >= min
    | Max max -> int <= max

let reasonInteger = function
    | Min min -> sprintf "must be at least %d" min
    | Max max -> sprintf "must be at most %d" max

let verifyInteger value prop name =
    if testInteger value prop then Choice1Of2 value
    else sprintf "'%s' %s" name (reasonInteger prop) |> Choice2Of2

type TextFieldName = TextFieldName of string
type DecimalFieldName= DecimalFieldName of string
type IntegerFieldName= IntegerFieldName of string

type FormField = 
    | TextField of TextFieldName * TextFieldProperty list
    | DecimalField of DecimalFieldName * DecimalFieldProperty list
    | IntegerField of IntegerFieldName * IntegerFieldProperty list

type FormResult = {
    GetText : TextFieldName -> string
    GetDecimal : DecimalFieldName -> decimal
    GetInteger : IntegerFieldName -> int
}

type ServerSideValidation = FormResult -> bool * string

type Form<'a> = 
    | Form of FormField list * ServerSideValidation list


let verify (field, value : obj) = 
    match (field, value) with
    | TextField ((TextFieldName name), props), (:? string as value) ->
        props
        |> List.fold
            (fun value prop ->
                value |> Choice.bind (fun value -> verifyText value prop name))
            (Choice1Of2 value)
        |> Choice.map box
    | DecimalField ((DecimalFieldName name), props), (:? decimal as value) ->
        props
        |> List.fold
            (fun value prop ->
                value |> Choice.bind (fun value -> verifyDecimal value prop name))
            (Choice1Of2 value)
        |> Choice.map box
    | IntegerField ((IntegerFieldName name), props), (:? int as value) ->
        props
        |> List.fold
            (fun value prop ->
                value |> Choice.bind (fun value -> verifyInteger value prop name))
            (Choice1Of2 value)
        |> Choice.map box
    | _, v -> failwithf "Unexpected type %s" (v.GetType().FullName)

let name = function
    | TextField ((TextFieldName name), _) -> name
    | DecimalField ((DecimalFieldName name), _) -> name
    | IntegerField ((IntegerFieldName name), _) -> name

let parse = function
    | TextField _, value -> Choice1Of2 value |> Choice.map box
    | DecimalField _, value -> Parse.decimal value |> Choice.map box
    | IntegerField _, value -> Parse.int32 value |> Choice.map box

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
                GetText = fun (TextFieldName key) -> map.[key] :?> _
                GetDecimal = fun (DecimalFieldName key) -> map.[key] :?> _
                GetInteger = fun (IntegerFieldName key) -> map.[key] :?> _
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