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
let index = __SOURCE_DIRECTORY__ ++ "index.tex"
let texFile = outputDir ++ "index.tex"

let createPDF fileName =
    use p = new System.Diagnostics.Process()
    p.StartInfo.FileName <- "pdflatex.exe"
    p.StartInfo.Arguments <- sprintf "-output-directory=%s %s" (Path.GetDirectoryName(fileName)) fileName
    p.StartInfo.UseShellExecute <- false
    p.StartInfo.RedirectStandardOutput <- false
    p.Start() |> ignore
    p.WaitForExit()
    for ext in ["aux"; "out"; "log"] do
        let auxFile = Path.ChangeExtension(fileName, ext)
        printfn "Delete auxiliary file: %s" auxFile
        if File.Exists(auxFile) then File.Delete(auxFile)
    if p.ExitCode <> 0 then exit p.ExitCode
    Path.ChangeExtension(fileName, "pdf")

let numberSections filePath =
    let contents = File.ReadAllText(filePath)
    let replaced = contents.Replace(@"section*{", @"section{")
    File.WriteAllText(filePath, replaced)

CreateDir outputDir

["streszczenie";"abstract";"chapter1-intro"]
|> List.iter (fun x -> 
                Literate.ProcessMarkdown(
                        __SOURCE_DIRECTORY__ ++ x + ".md", 
                        __SOURCE_DIRECTORY__ ++ ".." ++ "templates" ++ "template.tex", 
                        outputDir ++ x + ".tex", 
                        format = OutputKind.Latex)
                numberSections (outputDir ++ x + ".tex"))

File.Copy(index, texFile, true) |> ignore
let pdf = createPDF texFile

let newTempFile = Path.GetTempFileName()
let pdfTempFile = Path.ChangeExtension(newTempFile, "pdf")
File.Move(newTempFile, pdfTempFile)
File.Copy(pdf, pdfTempFile, true)
System.Diagnostics.Process.Start(pdfTempFile)