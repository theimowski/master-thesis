open Suave
open Suave.Http.Successful
open Suave.Web
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.RequestErrors
open Suave.Types

open MusicStore.Models

let registerSafeType t = 
    let fields = Reflection.FSharpType.GetRecordFields(t) |> Array.map (fun f -> f.Name)
    DotLiquid.Template.RegisterSafeType(t, fields)

typeof<Album>.DeclaringType.GetNestedTypes()
|> Array.iter registerSafeType

let contents = System.IO.File.ReadAllText("index.html")
let index = DotLiquid.Template.Parse(contents)
let store = DotLiquid.Template.Parse(System.IO.File.ReadAllText("store.html"))
let album = DotLiquid.Template.Parse(System.IO.File.ReadAllText("album.html"))
let genre = DotLiquid.Template.Parse(System.IO.File.ReadAllText("genre.html"))
let manageIndex = DotLiquid.Template.Parse(System.IO.File.ReadAllText("manage_index.html"))

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
              DatabaseVendor = Common.DatabaseProviderTypes.MSSQLSERVER>

let ctx = sql.GetDataContext()

let getStore = 
    fun (x: HttpContext) -> async {     
        let genres = query {
            for g in ctx.``[dbo].[Genre]`` do
            select g.Name
        } 

        return! partial({Genres = genres |> Seq.toArray |> Array.map Uri.EscapeDataString}, store) x
    }

let getGenre(name) = 
    fun (x: HttpContext) -> async {
        let albums = query {
            for a in ctx.``[dbo].[Album]`` do
            where (a.GenreId = query {
                for g in ctx.``[dbo].[Genre]`` do 
                where (g.Name = name)
                select g.GenreId
                exactlyOne
            })
            select {AlbumBrief.Id = a.AlbumId; Title = a.Title }
        }

        return! partial({Name = name; Albums = albums |> Seq.toArray}, genre) x
    }

let getAlbum(id) = 
    fun (x: HttpContext) -> async {
        let a = query {
            for a in ctx.``[dbo].[Album]`` do
            where (a.AlbumId = id)
            select {AlbumBrief.Id = a.AlbumId; Title = a.Title}
            exactlyOne
        }

        return! partial(a, album) x
    }

let manage = 
    fun (x: HttpContext) -> async {
        let albums = query {
            for album in ctx.``[dbo].[Album]`` do 
            join artist in ctx.``[dbo].[Artist]`` on (album.ArtistId = artist.ArtistId)
            join genre in ctx.``[dbo].[Genre]`` on (album.GenreId = genre.GenreId)
            select { 
                Album.Id = album.AlbumId
                Title = album.Title
                Artist = artist.Name
                Genre = genre.Name
                Price = album.Price
            }
        }

        return! partial({Albums = albums |> Seq.toArray}, manageIndex) x
    }

let q = Suave.Types.HttpRequest.query'

choose [
    GET >>= choose [
        url "/" >>= (HTML "Home Page")
        url "/store" >>= getStore
        url "/store/browse" >>= request(fun x -> cond (q x "genre") getGenre never)
        url_scan "/store/details/%d" getAlbum

        url "/store/manage" >>= manage

        url_regex "(.*?)\.(?!js$|css$|png$).*" >>= RequestErrors.FORBIDDEN "Access denied."
        Files.browse'
    ]

    NOT_FOUND "404"
]
|> web_server default_config 