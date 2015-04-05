module MusicStore.FormUtils2

open System
open Microsoft.FSharp.Reflection
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

open Suave
open Suave.Html
open Suave.Utils
open Suave.Types

type ServerSideMsg = string
type HtmlAttribute = string * string
type Validation<'a> = ('a -> bool) * ServerSideMsg * HtmlAttribute

type Property<'a, 'b> = ('a -> Expr<'b>) * Validation<'b> list

type FormProp<'a> =
    | StringProp of Property<'a, string>
    | DecimalProp of Property<'a, decimal>
    | IntProp of Property<'a, int>

type Form<'a> = Form of FormProp<'a> list

let formatDec (d : Decimal) = d.ToString(Globalization.CultureInfo.InvariantCulture)

let maxLength max : Validation<string> = (fun s -> s.Length <= max), "is to looong", ("maxlength", max.ToString())
let step step : Validation<decimal> = (fun d -> d % step = 0.0M), "must be multiply", ("step", formatDec step)



let isOptional (typ : Type) =
        typ.IsGenericType && 
        typ.GetGenericTypeDefinition() = typedefof<option<_>>

let parse = function
| t, "" when isOptional t -> Choice1Of2 None |> Choice.map box
| t, value when isOptional t -> 
    let genT = t.GetGenericArguments().[0]
    match genT, value with
    | t, value when t = typeof<String> -> Choice1Of2 value |> Choice.map (Some >> box)
    | t, value when t = typeof<Decimal> -> Suave.Model.Parse.decimal value |> Choice.map (Some >> box)
    | t, value when t = typeof<Int32> -> Suave.Model.Parse.int32 value |> Choice.map (Some >> box)
    | t, _ -> failwithf "not supported type: %s" t.FullName

| t, value when t = typeof<String> -> Choice1Of2 value |> Choice.map box
| t, value when t = typeof<Decimal> -> Suave.Model.Parse.decimal value |> Choice.map box
| t, value when t = typeof<Int32> -> Suave.Model.Parse.int32 value |> Choice.map box
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
    | StringProp (q, _) -> getName q
    | DecimalProp (q, _) -> getName q
    | IntProp (q, _) -> getName q

let thrd (_,_,x) = x

let getHtmlAttrs = function
    | StringProp (_,xs) -> xs |> List.map thrd
    | DecimalProp (_,xs) -> xs |> List.map thrd
    | IntProp (_,xs) -> xs |> List.map thrd

let getHtmlProps (Form props) (quotF : 'a -> Expr<'b>) : (string * string) list =
    props
    |> List.filter (fun p -> getName quotF = (p |> getQuotName))
    |> List.collect (fun p -> p |> getHtmlAttrs)

let validate' = function
    | StringProp p, v -> validate (p, v)
    | DecimalProp p, v -> validate (p, v)
    | IntProp p, v -> validate (p, v)

let bindRequest<'a> (form : Form<'a>) (req : HttpRequest) =
    let bindForm key = Model.Binding.form key Choice1Of2
    let t = form.GetType().GetGenericArguments().[0]
    let props = t.GetProperties() |> Array.toList
    let types = props |> List.map (fun p -> p.PropertyType)

    let getValue (prop : Reflection.PropertyInfo) =
        if isOptional (prop.PropertyType) then
            defaultArg (req.formData prop.Name) "" |> Choice1Of2
        else 
            req |> bindForm prop.Name

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
        let (Form props) = form
        props
        |> List.fold 
            (fun record prop ->
                record |> Choice.bind (fun record ->
                    validate' (prop, record)))
            (Choice1Of2 record)
    ))

let input<'a, 'b> (form : Form<'a>) (quotF : 'a -> Expr<'b>) typ required attrs =
    let name = getName quotF
    let props = getHtmlProps form quotF
    let required = if required then ["required",""] else []
    inputAttr (["name", name; "type", typ]
                @ required
                @ attrs
                @ (props))

let textInput<'a> form quotF = input<'a, string> form quotF "text" true
let optionalTextInput<'a> form quotF = input<'a, string option> form quotF "text" false
let decimalInput<'a> form quotF = input<'a, decimal> form quotF "number" true
let passwordInput<'a> form quotF = input<'a, string> form quotF "password" true


type Logon = {
    UserName : string
    Price : decimal
    Age : int
    Details : string option
}

let logonForm = Form [ StringProp ((fun f -> <@ f.UserName @>), [ maxLength 4 ]) 
                       DecimalProp ((fun f -> <@ f.Price @>), [ step 0.01M ]) ]

let bi = bindRequest logonForm 

let h = passwordInput logonForm (fun f -> <@ f.UserName @>) []
let h2 = decimalInput logonForm (fun f -> <@ f.Price @>) []