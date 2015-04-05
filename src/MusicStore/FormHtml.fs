module MusicStore.FormHtml

open System

open Suave.Html

open MusicStore.FormUtils

let formatDec (d : Decimal) = d.ToString(Globalization.CultureInfo.InvariantCulture)

let textAttr = function
    | MaxLength max -> "maxlength", max.ToString()
    
let passwordAttr = function
    | MaxPassLength max -> "maxlength", max.ToString()

let decimalAttr = function
    | Minimum min -> "min", formatDec min
    | Maximum max -> "max", formatDec max
    | Step step -> "step", formatDec step

let textInput (TextField(name, props)) attrs =
    inputAttr (["name", name; "type", "text"; "required", ""]
                @ attrs
                @ (props |> List.map textAttr))

let passwordInput (PasswordField(name, props)) attrs = 
    inputAttr (["name", name; "type", "password"; "required", ""]
                @ attrs
                @ (props |> List.map passwordAttr))

let decimalInput (DecimalField(name, props)) attrs = 
    inputAttr (["name", name; "type", "number"; "required", ""] 
                @ attrs
                @ (props |> List.map decimalAttr))

