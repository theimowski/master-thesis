module MusicStore.FormHtml

open System

open Suave.Html

open MusicStore.FormUtils

let formatDec (d : Decimal) = d.ToString(Globalization.CultureInfo.InvariantCulture)

let decimalAttr = function
    | Minimum min -> "min", formatDec min
    | Maximum max -> "max", formatDec max
    | Step step -> "step", formatDec step

let textAttr = function
    | MaxLength max -> "maxlength", max.ToString()

let decimalInput (DecimalField(name, props)) attrs = 
    inputAttr (["name", name; "type", "number"; "required", ""] 
                @ attrs
                @ (props |> List.map decimalAttr))

let textInput (TextField(name, props)) attrs =
    inputAttr (["name", name; "type", "text"; "required", ""]
                @ attrs
                @ (props |> List.map textAttr))