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
let albumCreate = DotLiquid.Template.Parse(System.IO.File.ReadAllText("album_create.html"))
let albumEdit = DotLiquid.Template.Parse(System.IO.File.ReadAllText("album_edit.html"))
let albumDelete = DotLiquid.Template.Parse(System.IO.File.ReadAllText("album_delete.html"))

open System
open System.Data
open System.Linq
open FSharp.Data.Sql

type sql = SqlDataProvider< 
              "Server=(LocalDb)\\v11.0;Database=MusicStore;Trusted_Connection=True;MultipleActiveResultSets=true",
              DatabaseVendor = Common.DatabaseProviderTypes.MSSQLSERVER>

let ctx = sql.GetDataContext()

let HTML(container) = 
    fun (x : HttpContext) -> async {
        let genres = query {
            for g in ctx.``[dbo].[Genre]`` do
            select (Uri.EscapeDataString g.Name)
        } 

        let model = {Index.Container = container; Genres = genres |> Seq.toArray}

        return! OK (index.Render(DotLiquid.Hash.FromAnonymousObject(model))) x
    }

let partial(model,template : DotLiquid.Template) =
    fun (x : HttpContext) -> async {
        return! HTML (template.Render(DotLiquid.Hash.FromAnonymousObject(model))) x
    }

let getStore = 
    fun (x: HttpContext) -> async {     
        let genres = query {
            for g in ctx.``[dbo].[Genre]`` do
            select (Uri.EscapeDataString g.Name)
        } 

        return! partial({Store.Genres = genres |> Seq.toArray}, store) x
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
            for album in ctx.``[dbo].[Album]`` do 
            where (album.AlbumId = id)
            join artist in ctx.``[dbo].[Artist]`` on (album.ArtistId = artist.ArtistId)
            join genre in ctx.``[dbo].[Genre]`` on (album.GenreId = genre.GenreId)
            select { 
                Album.Id = album.AlbumId
                Title = album.Title
                Artist = artist.Name
                Genre = genre.Name
                Price = album.Price.ToString(Globalization.CultureInfo.InvariantCulture)
                Art = album.AlbumArtUrl
            }
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
                Price = album.Price.ToString(Globalization.CultureInfo.InvariantCulture)
                Art = album.AlbumArtUrl
            }
        }

        return! partial({Albums = albums |> Seq.toArray}, manageIndex) x
    }

let getCreateAlbum = 
    fun (x: HttpContext) -> async { 
        let genres = query {
            for g in ctx.``[dbo].[Genre]`` do select {GenreBrief.Id = g.GenreId; Name = g.Name}
        }

        let artists = query {
            for a in ctx.``[dbo].[Artist]`` do select {ArtistBrief.Id = a.ArtistId; Name = a.Name}
        }

        return! partial({CreateAlbum.Genres = genres |> Seq.toArray; Artists = artists |> Seq.toArray}, albumCreate) x
    }

let getEditAlbum(id) =
    fun (x: HttpContext) -> async { 
        let genres = query {
            for g in ctx.``[dbo].[Genre]`` do select {GenreBrief.Id = g.GenreId; Name = g.Name}
        }

        let artists = query {
            for a in ctx.``[dbo].[Artist]`` do select {ArtistBrief.Id = a.ArtistId; Name = a.Name}
        }

        let album = query {
            for a in ctx.``[dbo].[Album]`` do
            where (a.AlbumId = id)
            select {
                Album.Id = a.AlbumId
                Title = a.Title
                Price = a.Price.ToString(Globalization.CultureInfo.InvariantCulture)
                Art = a.AlbumArtUrl
                Artist = artists |> Seq.find (fun artist -> artist.Id = a.ArtistId) |> (fun a -> a.Name)
                Genre = genres |> Seq.find (fun g -> g.Id = a.GenreId) |> (fun g -> g.Name)
            }
            exactlyOne
        }

        return! partial({EditAlbum.Genres = genres |> Seq.toArray; Artists = artists |> Seq.toArray; Album = album}, albumEdit) x
    }

let getDeleteAlbum(id) = 
    fun (x: HttpContext) -> async { 
        let title = query {
            for a in ctx.``[dbo].[Album]`` do
            where (a.AlbumId = id)
            select a.Title
            exactlyOne
        }

        return! partial({AlbumBrief.Id = id; Title = title}, albumDelete) x
    }

let f = Suave.Types.HttpRequest.form'
let mint x = 
    match x |> Int32.TryParse with
    | true, v -> Some v
    | _ -> None
let mdec x = 
    match Decimal.TryParse(x, Globalization.NumberStyles.AllowDecimalPoint, Globalization.CultureInfo.InvariantCulture) with
    | true, v -> Some v
    | _ -> None

let postCreateAlbum = 
    fun (x: HttpContext) -> async {
        let artist = f x.request "artist" |> Option.bind mint
        let genre = f x.request "genre" |> Option.bind mint
        let price = f x.request "price" |> Option.bind mdec
        let title = f x.request "title"
        let artUrl = f x.request "artUrl"

        match artist,genre,title,price,artUrl with
        | Some artist, Some genre, Some title, Some price, Some artUrl ->
            let album = ctx.``[dbo].[Album]``.Create()
            album.ArtistId <- artist
            album.GenreId <- genre
            album.Title <- title
            album.Price <- price
            album.Created <- DateTime.UtcNow
            album.AlbumArtUrl <- artUrl

            ctx.SubmitUpdates()

            return! Redirection.redirect "/store/manage" x    
        | _ -> return! BAD_REQUEST "malformed POST" x
    }

let postEditAlbum(id) = 
    fun (x: HttpContext) -> async {
        let artist = f x.request "artist" |> Option.bind mint
        let genre = f x.request "genre" |> Option.bind mint
        let price = f x.request "price" |> Option.bind mdec
        let title = f x.request "title"
        let artUrl = f x.request "artUrl"

        match artist,genre,title,price,artUrl with
        | Some artist, Some genre, Some title, Some price, Some artUrl ->
            let album = query {
                for a in ctx.``[dbo].[Album]`` do
                where (a.AlbumId = id)
                select a
                exactlyOne
            }
            album.ArtistId <- artist
            album.GenreId <- genre
            album.Title <- title
            album.Price <- price
            album.Created <- DateTime.UtcNow
            album.AlbumArtUrl <- artUrl

            ctx.SubmitUpdates()

            return! Redirection.redirect "/store/manage" x    
        | _ -> return! BAD_REQUEST "malformed POST" x
    }

let postDeleteAlbum(id) =
    fun (x: HttpContext) -> async { 
        let album = query {
            for a in ctx.``[dbo].[Album]`` do
            where (a.AlbumId = id)
            select a
            exactlyOne
        }

        album.Delete()
        ctx.SubmitUpdates()

        return! Redirection.redirect "/store/manage" x    
    }

let q = Suave.Types.HttpRequest.query'

choose [
    GET >>= choose [
        url "/" >>= (HTML """<div id="promotion"/>""")
        url "/store" >>= getStore
        url "/store/browse" >>= request(fun x -> cond (q x "genre") getGenre never)
        url_scan "/store/details/%d" getAlbum

        url "/store/manage" >>= manage
        url "/store/manage/create" >>= getCreateAlbum
        url_scan "/store/manage/edit/%d" getEditAlbum
        url_scan "/store/manage/delete/%d" getDeleteAlbum

        url_regex "(.*?)\.(?!js$|css$|png$).*" >>= RequestErrors.FORBIDDEN "Access denied."
        Files.browse'
    ]

    POST >>= ParsingAndControl.parse_post_data >>= choose [
        url "/store/manage/create" >>= postCreateAlbum
        url_scan "/store/manage/edit/%d" postEditAlbum
        url_scan "/store/manage/delete/%d" postDeleteAlbum
    ]

    NOT_FOUND "404"
]
|> web_server default_config 