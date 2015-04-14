module MusicStore.FormUtils

open System
open System.Text.RegularExpressions
open Microsoft.FSharp.Reflection
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

open Suave
open Suave.Html
open Suave.Utils
open Suave.Types

[<Literal>]
let emailPattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z"

type Email = Email of string with 
    static member Create s = 
        if Regex.IsMatch(s, emailPattern, RegexOptions.IgnoreCase) then
            Choice1Of2 (Email s)
        else
            Choice2Of2 (sprintf "%s is not a valid email" s)

type Password = Password of string

type ServerSideMsg = string
type HtmlAttribute = string * string
type Validation<'a> = ('a -> bool) * ServerSideMsg * HtmlAttribute
type ServerSideValidation<'a> = ('a -> bool) * ServerSideMsg

type Property<'a, 'b> = ('a -> Expr<'b>) * Validation<'b> list

type FormProp<'a> =
    | TextProp of Property<'a, string>
    | PasswordProp of Property<'a, Password>
    | DecimalProp of Property<'a, decimal>

type Form<'a> = Form of FormProp<'a> list * ServerSideValidation<'a> list

let formatDec (d : Decimal) = d.ToString(Globalization.CultureInfo.InvariantCulture)

let maxLength max : Validation<string> = 
    (fun s -> s.Length <= max), 
    (sprintf "must be at most %d characters" max), 
    ("maxlength", max.ToString())

let matches pattern : Validation<string> =
    (fun p -> Regex(pattern).IsMatch(p)),
    (sprintf "doesn't match pattern %s" pattern),
    ("pattern", pattern)

let passwordRegex pattern : Validation<Password> =
    (fun (Password p) -> Regex(pattern).IsMatch(p)),
    (sprintf "doesn't match pattern %s" pattern),
    ("pattern", pattern)

let min min : Validation<decimal> =
    (fun d -> d >= min),
    (sprintf "must be at least %M" min),
    ("min", formatDec min)

let max max : Validation<decimal> =
    (fun d -> d <= max),
    (sprintf "must be at most %M" max),
    ("min", formatDec max)

let step step : Validation<decimal> = 
    (fun d -> d % step = 0.0M), 
    (sprintf "must be a multiply of %M" step), 
    ("step", formatDec step)

let (|Optional|_|) (typ : Type) = 
    if typ.IsGenericType 
       && typ.GetGenericTypeDefinition() = typedefof<option<_>> then 
        Some(typ.GetGenericArguments().[0])
    else None
    
let parse = function
| Optional(t), "" -> Choice1Of2 None |> Choice.map box
| Optional(genT), value -> 
    match genT, value with
    | t, value when t = typeof<String> -> Choice1Of2 value |> Choice.map (Some >> box)
    | t, value when t = typeof<Password> -> Password value |> Choice1Of2 |> Choice.map (Some >> box)
    | t, value when t = typeof<Decimal> -> Suave.Model.Parse.decimal value |> Choice.map (Some >> box)
    | t, value when t = typeof<Email> -> Email.Create value |> Choice.map (Some >> box)
    | t, _ -> failwithf "not supported type: %s" t.FullName

| t, value when t = typeof<String> -> Choice1Of2 value |> Choice.map box
| t, value when t = typeof<Password> -> Password value |> Choice1Of2 |> Choice.map box
| t, value when t = typeof<Decimal> -> Suave.Model.Parse.decimal value |> Choice.map box
| t, value when t = typeof<Email> -> Email.Create value |> Choice.map box
| t, _ -> failwithf "not supported type: %s" t.FullName

let validateSingle ((quotF, ((test, msg, _) : Validation<'b>)), value : 'a) =
    match quotF value with
    | PropertyGet (Some (Value (:? 'a as v, _)), p, _) -> 
        let propVal = p.GetGetMethod().Invoke(v, [||]) :?> 'b
        if test propVal then Choice1Of2 value
        else Choice2Of2 (sprintf "%s %s" p.Name msg)
    | _ -> failwith "unrecognized quotation"

let getName (quotF : 'a -> Expr<'b>) =
    let n = Unchecked.defaultof<'a>
    match quotF n with
    | PropertyGet (_, p, _) -> 
        p.Name
    | _ -> failwith "unrecognized quotation"

let validate ((quotF, validations), value : 'a) =
    validations
    |> List.fold
        (fun value validation ->
            value |> Choice.bind (fun value -> validateSingle ((quotF, validation), value)))
        (Choice1Of2 value)    

let getQuotName = function 
    | TextProp (q, _) -> getName q
    | PasswordProp (q, _) -> getName q
    | DecimalProp (q, _) -> getName q

let thrd (_,_,x) = x

let getHtmlAttrs = function
    | TextProp (_,xs) -> xs |> List.map thrd
    | PasswordProp (_, xs) -> xs |> List.map thrd
    | DecimalProp (_,xs) -> xs |> List.map thrd

let getHtmlProps (Form (props,_)) (quotF : 'a -> Expr<'b>) : (string * string) list =
    props
    |> List.filter (fun p -> getName quotF = (p |> getQuotName))
    |> List.collect (fun p -> p |> getHtmlAttrs)

let validate' = function
    | TextProp p, v -> validate (p, v)
    | PasswordProp p, v -> validate (p, v)
    | DecimalProp p, v -> validate (p, v)

let bindForm<'a> (form : Form<'a>) (req : HttpRequest) =
    let bindForm key = Model.Binding.form key Choice1Of2
    let t = form.GetType().GetGenericArguments().[0]
    let props = t.GetProperties() |> Array.toList
    let types = props |> List.map (fun p -> p.PropertyType)

    let getValue (prop : Reflection.PropertyInfo) =
        match prop.PropertyType with
        | Optional(_) -> defaultArg (req.formData prop.Name) "" |> Choice1Of2
        | _ -> req |> bindForm prop.Name

    let values =
        props 
        |> List.fold
            (fun list prop ->
                list |> Choice.bind (fun xs ->
                    getValue prop
                    |> Choice.map (fun x -> x :: xs)))
            (Choice1Of2 [])
        |> Choice.map List.rev

    values 
    |> Choice.bind (fun values -> 
    (types, values)
    ||> List.zip
    |> List.fold
        (fun list (typ,value) ->
            list |> Choice.bind (fun xs -> 
                parse (typ,value)
                |> Choice.map (fun x -> x :: xs)))
        (Choice1Of2 [])
    |> Choice.map (List.rev >> List.toArray >> (fun objs -> FSharpValue.MakeRecord(t, objs) :?> 'a))
    |> Choice.bind (fun record ->
        let (Form (props,validations)) = form
        props
        |> List.fold 
            (fun record prop ->
                record |> Choice.bind (fun record ->
                    validate' (prop, record)))
            (Choice1Of2 record)
        |> Choice.bind (fun record ->
            validations
            |> List.fold
                (fun record (validation, msg) ->
                    record |> Choice.bind (fun record ->
                        if validation record then
                            Choice1Of2 record
                        else
                            Choice2Of2 msg))
                (Choice1Of2 record)
        )
    ))