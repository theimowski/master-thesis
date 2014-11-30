#r "packages/FAKE/tools/FakeLib.dll" 
open Fake 

let (++) a b = System.IO.Path.Combine(a,b)

Target "GenerateDocs" (fun _ ->
    executeFSIWithArgs (__SOURCE_DIRECTORY__ ++ "docs") "generate.fsx" ["--define:RELEASE"] [] |> ignore
)

RunTargetOrDefault "GenerateDocs"