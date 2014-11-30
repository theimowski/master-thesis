#I "../packages/FSharp.Formatting/lib/net40"
#I "../packages/Microsoft.AspNet.Razor/lib/net40"
#I "../packages/RazorEngine/lib/net40"
#I "../packages/FSharp.Compiler.Service/lib/net40"
#r "System.Web.dll"
#r "FSharp.Markdown.dll"
#r "FSharp.CodeFormat.dll"
#r "FSharp.Literate.dll"
#r "FSharp.MetadataFormat.dll"
#r "System.Web.Razor.dll"
#r "RazorEngine.dll"
#r "FSharp.Compiler.Service.dll"

#r "../packages/FAKE/tools/FakeLib.dll"

open System
open System.IO

open Fake
open FSharp.Literate

let (++) a b = Path.Combine(a,b)

let outputDir = __SOURCE_DIRECTORY__ ++ ".." ++ "output"
let input = __SOURCE_DIRECTORY__ ++ "index.md"
let template =  __SOURCE_DIRECTORY__ ++ ".." ++ "templates" ++ "template-color.tex"
let texFile = outputDir ++ "index.tex"

let createPDF fileName =
    use p = new System.Diagnostics.Process()
    p.StartInfo.FileName <- "pdflatex.exe"
    p.StartInfo.Arguments <- sprintf "-output-directory=%s %s" (Path.GetDirectoryName(fileName)) fileName
    p.StartInfo.UseShellExecute <- false
    p.StartInfo.RedirectStandardOutput <- false
    p.Start() |> ignore
    p.WaitForExit()
    for ext in ["tex"; "aux"; "out"; "log"] do
        let auxFile = Path.ChangeExtension(fileName, ext)
        printfn "Delete auxiliary file: %s" auxFile
        File.Delete(auxFile)

CreateDir outputDir

Literate.ProcessMarkdown(input, template, texFile, format = OutputKind.Latex)
createPDF texFile