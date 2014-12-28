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

open System
open System.Data
open System.Linq
open FSharp.Data.Sql

type sql = SqlDataProvider< 
              "Server=(LocalDb)\\v11.0;Database=MusicStore;Trusted_Connection=True;MultipleActiveResultSets=true",
              DatabaseVendor = Common.DatabaseProviderTypes.MSSQLSERVER,
              IndividualsAmount = 1000,
              UseOptionTypes = true >

let getStore = 
    fun (x: HttpContext) -> async {     
        let ctx = sql.GetDataContext()
        
        let genres = query {
            for g in ctx.``[dbo].[Genre]`` do
            where g.Name.IsSome
            select {Name = g.Name.Value}
        } 

        return! partial({Genres = genres |> Seq.toArray}, store) x
    }

choose [
    GET >>= choose [
        url "/" >>= (HTML "Home Page")
        url "/store" >>= getStore
        url "/store/browse" 
            >>= request(fun request -> cond (HttpRequest.query(request) ^^ "genre") 
                                            (fun name -> partial ({Name = name}, genre)) 
                                            never)
        url_scan "/store/details/%d" (fun id -> partial({Title = "Album " + id.ToString()}, album))
        
        url_regex "(.*?)\.(?!js$|css$).*" >>= RequestErrors.FORBIDDEN "Access denied."
        Files.browse'
    ]

    NOT_FOUND "404"
]
|> web_server default_config 