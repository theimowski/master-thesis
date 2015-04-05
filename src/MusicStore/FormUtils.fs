module MusicStore.FormUtils

open System

open Suave.Model
open Suave.Utils

let bindForm key = Binding.form key Choice1Of2

type TextFieldProperty =
    | MaxLength of int

type DecimalFieldProperty =
    | Minimum of decimal
    | Maximum of decimal
    | Step of decimal

type IntegerFieldProperty =
    | Min of int
    | Max of int

type TextField    = TextField of string * TextFieldProperty list
    with member this.Name = match this with TextField (n, _) -> n
type DecimalField = DecimalField of string * DecimalFieldProperty list
    with member this.Name = match this with DecimalField (n, _) -> n
type IntegerField = IntegerField of string * IntegerFieldProperty list
    with member this.Name = match this with IntegerField (n, _) -> n

type FormField =
    | TextFormField of TextField
    | DecimalFormField of DecimalField
    | IntegerFormField of IntegerField

type FormResult = {
    GetText : TextField -> string
    GetDecimal : DecimalField -> decimal
    GetInteger : IntegerField -> int
}

type ServerSideValidation = FormResult -> bool * string

type Form = {
    Fields : FormField list
    ServerSideValidations : ServerSideValidation list
}

let testText (text : string) = function
    | MaxLength len -> text.Length <= len

let reasonText = function
    | MaxLength len -> sprintf "must be at most %d characters" len

let testDecimal (decimal : decimal) = function
    | Minimum min -> decimal >= min
    | Maximum max -> decimal <= max
    | Step step -> decimal % step = 0.0M

let reasonDecimal = function
    | Minimum min -> sprintf "must be at least %M" min
    | Maximum max -> sprintf "must be at most %M" max
    | Step step -> sprintf "must be a multiply of %M" step

let testInteger (int : int) = function
    | Min min -> int >= min
    | Max max -> int <= max

let reasonInteger = function
    | Min min -> sprintf "must be at least %d" min
    | Max max -> sprintf "must be at most %d" max

let verify (name,props) (testF, reasonF) value =
    let verify' prop = 
        if testF value prop then Choice1Of2 value
        else sprintf "'%s' %s" name (reasonF prop) |> Choice2Of2

    props
    |> List.fold
        (fun value prop ->
            value |> Choice.bind (fun value -> verify' prop))
        (Choice1Of2 value)

let verifyText (TextField (n,ps)) = verify (n,ps) (testText, reasonText) 
let verifyDecimal (DecimalField (n,ps)) = verify (n,ps) (testDecimal, reasonDecimal) 
let verifyInteger (IntegerField (n,ps)) = verify (n,ps) (testInteger, reasonInteger)

let name = function
    | TextFormField (TextField (name, _)) -> name
    | DecimalFormField (DecimalField (name, _)) -> name
    | IntegerFormField (IntegerField (name, _)) -> name

let parseAndVerify = function
    | TextFormField f, value -> Choice1Of2 value >>. verifyText f |> Choice.map box
    | DecimalFormField f, value -> Parse.decimal value >>. verifyDecimal f |> Choice.map box
    | IntegerFormField f, value -> Parse.int32 value >>. verifyInteger f |> Choice.map box

let bindingForm form req =
    let fields = form.Fields
    let validations = form.ServerSideValidations
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
                    parseAndVerify (field, value) 
                    |> Choice.map (fun x -> 
                        map |> Map.add (name field) x)))
            (Choice1Of2 Map.empty)
        |> Choice.map (fun map ->
            let result = {
                GetText = fun (TextField (name,_)) -> map.[name] :?> _
                GetDecimal = fun (DecimalField (name,_)) -> map.[name] :?> _
                GetInteger = fun (IntegerField (name,_)) -> map.[name] :?> _
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