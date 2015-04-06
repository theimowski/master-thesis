module MusicStore.FormHtml

open System
open Microsoft.FSharp.Quotations

open Suave.Html

open MusicStore.FormUtils

let input<'a, 'b> (form : Form<'a>) (quotF : 'a -> Expr<'b>) typ required attrs =
    let name = getName quotF
    let props = getHtmlProps form quotF
    let required = if required then ["required",""] else []
    inputAttr (["name", name; "type", typ]
                @ required
                @ attrs
                @ (props))

let textInput<'a> form quotF = input<'a, string> form quotF "text" true
let passwordInput<'a> form quotF = input<'a, string> form quotF "password" true
let emailInput<'a> form quotF = input<'a, Email> form quotF "email" true
let decimalInput<'a> form quotF = input<'a, decimal> form quotF "number" true
let integerInput<'a> form quotF attr = input<'a, int> form quotF "number" true (("step","1") :: attr)

let optionalTextInput<'a> form quotF = input<'a, string option> form quotF "text" false
let optionalEmailInput<'a> form quotF = input<'a, Email option> form quotF "email" false

let option value txt selected =
    if selected then
        tag "option" ["value", value; "selected", "selected"] (text txt)
    else
        tag "option" ["value", value] (text txt)

let format : (obj -> string) = function
    | :? string as s -> s
    | :? int as i -> string i
    | :? decimal as d -> formatDec d
    | t -> failwithf "unsupported type: %s" (t.GetType().FullName)

let selectInput<'a, 'b when 'b : equality> 
        (form : Form<'a>) 
        (quotF : 'a -> Expr<'b>) 
        (options : ('b * string) list)
        (selected : 'b option) =
    let x = 
        options
        |> List.map (fun (value,txt) -> option (format value) txt (selected = Some value))

    tag "select" ["name", getName quotF] (flatten x)
