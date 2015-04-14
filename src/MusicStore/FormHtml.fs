module MusicStore.FormHtml

open System
open Microsoft.FSharp.Quotations

open Suave.Html

open MusicStore.FormUtils

let option value txt selected =
    if selected then
        tag "option" ["value", value; "selected", "selected"] (text txt)
    else
        tag "option" ["value", value] (text txt)

let inputType = function
    | t when t = typeof<String> -> "text"
    | t when t = typeof<Password> -> "password"
    | t when t = typeof<Email> -> "email"
    | t when t = typeof<Decimal> -> "number"
    | t -> failwithf "not supported type: %s" t.FullName

let input<'a, 'b> (quotF : 'a -> Expr<'b>) attrs (form : Form<'a>) =
    let name = getName quotF
    let typ = 
        match typeof<'b> with
        | Optional(t) 
        | t -> inputType t 
    let required = 
        match typeof<'b> with
        | Optional(_) -> []
        | _ -> ["required",""]
    let props = getHtmlProps form quotF
    inputAttr (["name", name; "type", typ]
                @ required
                @ attrs
                @ props)

let format : (obj -> string) = function
    | :? string as s -> s
    | :? int as i -> string i
    | :? decimal as d -> formatDec d
    | t -> failwithf "unsupported type: %s" (t.GetType().FullName)

let selectInput<'a, 'b when 'b : equality> 
        (quotF : 'a -> Expr<'b>) 
        (options : ('b * string) list)
        (selected : 'b option) 
        (form : Form<'a>) =
    let x = 
        options
        |> List.map (fun (value,txt) -> option (format value) txt (selected = Some value))

    tag "select" ["name", getName quotF] (flatten x)