open Suave
open Suave.Http.Successful
open Suave.Web
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.RequestErrors
open Suave.Types

open MusicStore.Models

DotLiquid.Template.RegisterSafeType(typeof<Album>, [|"Id"; "Title"|])
DotLiquid.Template.RegisterSafeType(typeof<Genre>, [|"Name"; "Albums"|])

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
            select g.Name.Value
        } 

        return! partial({Genres = genres |> Seq.toArray |> Array.map Uri.EscapeDataString}, store) x
    }

let getGenre(name) = 
    fun (x: HttpContext) -> async {
        let ctx = sql.GetDataContext()

        let albums = query {
            for a in ctx.``[dbo].[Album]`` do
            where (a.GenreId = query {
                for g in ctx.``[dbo].[Genre]`` do 
                where (g.Name.IsSome && g.Name.Value = name)
                select g.GenreId
                exactlyOne
            } && a.Title.IsSome)
            select {Id = a.AlbumId; Title = a.Title.Value }
        }

        return! partial({Name = name; Albums = albums |> Seq.toArray}, genre) x
    }

let getAlbum(id) = 
    fun (x: HttpContext) -> async {
        let ctx = sql.GetDataContext()

        let a = query {
            for a in ctx.``[dbo].[Album]`` do
            where (a.AlbumId = id && a.Title.IsSome)
            select {Id = a.AlbumId; Title = a.Title.Value}
            exactlyOne
        }

        return! partial(a, album) x
    }

let q = Suave.Types.HttpRequest.query'

choose [
    GET >>= choose [
        url "/" >>= (HTML "Home Page")
        url "/store" >>= getStore
        url "/store/browse" >>= request(fun x -> cond (q x "genre") getGenre never)
        url_scan "/store/details/%d" getAlbum
        
        url_regex "(.*?)\.(?!js$|css$|png$).*" >>= RequestErrors.FORBIDDEN "Access denied."
        Files.browse'
    ]

    NOT_FOUND "404"
]
|> web_server default_config 