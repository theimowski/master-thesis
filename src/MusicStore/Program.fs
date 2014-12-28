open Suave                 // always open suave
open Suave.Http.Successful // for OK-result
open Suave.Web             // for config
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.RequestErrors
open Suave.Types

open MusicStore.Models

DotLiquid.Template.RegisterSafeType(typeof<Genre>, [|"Name"|])

let contents = System.IO.File.ReadAllText("index.html")
let index = DotLiquid.Template.Parse(contents)
let store = DotLiquid.Template.Parse(System.IO.File.ReadAllText("store.html"))
let album = DotLiquid.Template.Parse(System.IO.File.ReadAllText("album.html"))
let genre = DotLiquid.Template.Parse(System.IO.File.ReadAllText("genre.html"))

let HTML(container) = 
    fun (x : HttpContext) -> async {
        let dictionary = ["Container", container] |> dict
        return! OK (index.Render(DotLiquid.Hash.FromDictionary(dictionary))) x
    }

let partial(model,template : DotLiquid.Template) =
    fun (x : HttpContext) -> async {
        return! HTML (template.Render(DotLiquid.Hash.FromAnonymousObject(model))) x
    }

choose [
    GET >>= choose [
        url "/" >>= (HTML "Home Page")
        url "/store" >>= (partial ({Genres = [|{Name = "Rock"};{Name = "Disco"}|]}, store))
        url "/store/browse" 
            >>= request(fun request -> cond (HttpRequest.query(request) ^^ "genre") 
                                            (fun name -> partial ({Name = name}, genre)) 
                                            never)
        url_scan "/store/details/%d" (fun id -> partial({Title = "Album " + id.ToString()}, album))
        Files.browse'
    ]

    NOT_FOUND "404"
]
|> web_server default_config 