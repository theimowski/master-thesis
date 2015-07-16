#I "../packages/FSharp.Formatting/lib/net40"
#I "../packages/Microsoft.AspNet.Razor/lib/net45"
#I "../packages/RazorEngine/lib/net40"
#I "../packages/FSharp.Compiler.Service/lib/net40"
#I "../packages/FSharpVSPowerTools.Core/lib/net45"
#r "System.Web.dll"
#r "FSharp.Markdown.dll"
#r "FSharp.CodeFormat.dll"
#r "FSharp.Literate.dll"
#r "FSharp.MetadataFormat.dll"
#r "System.Web.Razor.dll"
#r "RazorEngine.dll"
#r "FSharp.Compiler.Service.dll"
#r "FSharpVSPowerTools.Core.dll"

#r "../packages/FAKE/tools/FakeLib.dll"

open System
open System.IO

open Fake
open FSharp.Literate
open System.Text.RegularExpressions

let (++) a b = Path.Combine(a,b)
let withExt ext path = Path.ChangeExtension(path, ext)
let fileName = Path.GetFileName

let outputDir = __SOURCE_DIRECTORY__ ++ ".." ++ "output"
let index = __SOURCE_DIRECTORY__ ++ "index.tex"
let texFile = outputDir ++ "index.tex"

let createPDF fileName =
    use p = new System.Diagnostics.Process()
    p.StartInfo.FileName <- "pdflatex.exe"
    p.StartInfo.Arguments <- sprintf " -output-directory=%s %s" (Path.GetDirectoryName(fileName)) fileName
    p.StartInfo.UseShellExecute <- false
    p.StartInfo.RedirectStandardOutput <- false
    p.Start() |> ignore
    p.WaitForExit()
    if p.ExitCode <> 0 then exit p.ExitCode
    Path.ChangeExtension(fileName, "pdf")

let numberSections filePath =
    let contents = File.ReadAllText(filePath)
    let replaced = contents.Replace(@"section*{", @"section{")
    let replaced = Regex.Replace(replaced, "\\\{\\\{\\\{([\w+, ]+)\\\}\\\}\\\}", "\cite{$1}")
    let replaced = Regex.Replace(replaced, "---([\w+ ]+)---", "\\begin{table}[h]\\caption{$1}\\centering\\setlength\\extrarowheight{2pt}")
    let replaced = replaced.Replace("\end{tabular}", "\end{tabular}\end{table}")

    let replaced = replaced.Replace( "\\begin{lstlisting}\n{", "\n\n\\begin{mylisting}[")
    let replaced = replaced.Replace( "\\end{lstlisting}", "\\end{mylisting}\n\n")
    let replaced = Regex.Replace(replaced, "\\\{\\\{([\w+, ]+)\\\}\\\}", "\\ref{$1}")

    File.WriteAllText(filePath, replaced)

CreateDir outputDir

Directory.EnumerateFiles(__SOURCE_DIRECTORY__)
|> Seq.filter (fun f -> Path.GetExtension(f) = ".md")
|> Seq.iter (fun f -> 
                Literate.ProcessMarkdown(
                        f, 
                        __SOURCE_DIRECTORY__ ++ ".." ++ "templates" ++ "template.tex", 
                        outputDir ++ (f |> fileName |> changeExt ".tex"), 
                        format = OutputKind.Latex))

Directory.EnumerateFiles(outputDir)
|> Seq.filter (fun f -> let f = Path.GetFileName(f) in f.StartsWith("section") && f.EndsWith(".tex"))
|> Seq.iter  numberSections

File.Copy(index, texFile, true) |> ignore
File.Copy(__SOURCE_DIRECTORY__ ++ "bibliography.bib", outputDir ++ "bibliography.bib", true)
let pdf = createPDF texFile

let newTempFile = Path.GetTempFileName()
let pdfTempFile = Path.ChangeExtension(newTempFile, "pdf")
File.Move(newTempFile, pdfTempFile)
File.Copy(pdf, pdfTempFile, true)
System.Diagnostics.Process.Start(pdfTempFile)