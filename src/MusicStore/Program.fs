module MusicStore.App

open Suave
open Suave.Http.Successful
open Suave.Web
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.RequestErrors
open Suave.Types

open MusicStore.View

open System
open System.Data
open System.Linq
open FSharp.Data.Sql

type sql = SqlDataProvider< 
              "Server=(LocalDb)\\v11.0;Database=MvcMusicStore;Trusted_Connection=True;MultipleActiveResultSets=true",
              DatabaseVendor = Common.DatabaseProviderTypes.MSSQLSERVER>

let ctx = sql.GetDataContext()

let HTML (model) =
    fun (x : HttpContext) -> async {
        let genres = query {
            for g in ctx.``[dbo].[Genres]`` do
            select (Uri.EscapeDataString g.Name)
        } 
        
        let container = View.render(model)
        let index = {Index.Container = container; Genres = genres |> Seq.toArray}

        return! (OK (View.render index) >>= Writers.set_mime_type "text/html; charset=utf-8") x
    }

let getHome =   
    fun (x: HttpContext) -> async {
        return! HTML({Placeholder = ()}) x
    }

let getStore = 
    fun (x: HttpContext) -> async {     
        let genres = query {
            for g in ctx.``[dbo].[Genres]`` do
            select (Uri.EscapeDataString g.Name)
        } 

        return! HTML({Store.Genres = genres |> Seq.toArray}) x
    }

let getGenre(name) = 
    fun (x: HttpContext) -> async {
        let albums = query {
            for a in ctx.``[dbo].[Albums]`` do
            where (a.GenreId = query {
                for g in ctx.``[dbo].[Genres]`` do 
                where (g.Name = name)
                select g.GenreId
                exactlyOne
            })
            select (a.AlbumId, a.Title)
        }

        return! HTML({Name = name; Albums = albums |> Seq.toArray}) x
    }

let getAlbum(id) = 
    fun (x: HttpContext) -> async {
        let a = query {
            for album in ctx.``[dbo].[Albums]`` do 
            where (album.AlbumId = id)
            join artist in ctx.``[dbo].[Artists]`` on (album.ArtistId = artist.ArtistId)
            join genre in ctx.``[dbo].[Genres]`` on (album.GenreId = genre.GenreId)
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

        return! HTML(a) x
    }

let manage = 
    fun (x: HttpContext) -> async {
        let albums = query {
            for album in ctx.``[dbo].[Albums]`` do 
            join artist in ctx.``[dbo].[Artists]`` on (album.ArtistId = artist.ArtistId)
            join genre in ctx.``[dbo].[Genres]`` on (album.GenreId = genre.GenreId)
            select { 
                Album.Id = album.AlbumId
                Title = album.Title
                Artist = artist.Name
                Genre = genre.Name
                Price = album.Price.ToString(Globalization.CultureInfo.InvariantCulture)
                Art = album.AlbumArtUrl
            }
        }

        return! HTML({Albums = albums |> Seq.toArray}) x
    }

let getCreateAlbum = 
    fun (x: HttpContext) -> async { 
        let genres = query {
            for g in ctx.``[dbo].[Genres]`` do select (g.GenreId, g.Name)
        }

        let artists = query {
            for a in ctx.``[dbo].[Artists]`` do select (a.ArtistId, a.Name)
        }

        return! HTML({CreateAlbum.Genres = genres |> Seq.toArray; Artists = artists |> Seq.toArray}) x
    }

let getEditAlbum(id) =
    fun (x: HttpContext) -> async { 
        let genres = query {
            for g in ctx.``[dbo].[Genres]`` do select (g.GenreId, g.Name)
        }

        let artists = query {
            for a in ctx.``[dbo].[Artists]`` do select (a.ArtistId,a.Name)
        }

        let album = query {
            for a in ctx.``[dbo].[Albums]`` do
            where (a.AlbumId = id)
            select {
                Album.Id = a.AlbumId
                Title = a.Title
                Price = a.Price.ToString(Globalization.CultureInfo.InvariantCulture)
                Art = a.AlbumArtUrl
                Artist = artists |> Seq.find (fun (id,_) -> id = a.ArtistId) |> snd
                Genre = genres |> Seq.find (fun (id,_) -> id = a.GenreId) |> snd
            }
            exactlyOne
        }

        return! HTML({EditAlbum.Genres = genres |> Seq.toArray; Artists = artists |> Seq.toArray; Album = album}) x
    }

let getDeleteAlbum(id) = 
    fun (x: HttpContext) -> async { 
        let title = query {
            for a in ctx.``[dbo].[Albums]`` do
            where (a.AlbumId = id)
            select a.Title
            exactlyOne
        }

        return! HTML({DeleteAlbum.Id = id; Title = title}) x
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
            let album = ctx.``[dbo].[Albums]``.Create()
            album.ArtistId <- artist
            album.GenreId <- genre
            album.Title <- title
            album.Price <- price
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
                for a in ctx.``[dbo].[Albums]`` do
                where (a.AlbumId = id)
                select a
                exactlyOne
            }
            album.ArtistId <- artist
            album.GenreId <- genre
            album.Title <- title
            album.Price <- price
            album.AlbumArtUrl <- artUrl

            ctx.SubmitUpdates()

            return! Redirection.redirect "/store/manage" x    
        | _ -> return! BAD_REQUEST "malformed POST" x
    }

let postDeleteAlbum(id) =
    fun (x: HttpContext) -> async { 
        let album = query {
            for a in ctx.``[dbo].[Albums]`` do
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
        url "/" >>= getHome
        url "/store" >>= getStore
        url "/store/browse" >>= request(fun x -> cond (q x "genre") getGenre never)
        url_scan "/store/details/%d" getAlbum

        url "/store/manage" >>= manage
        url "/store/manage/create" >>= getCreateAlbum
        url_scan "/store/manage/edit/%d" getEditAlbum
        url_scan "/store/manage/delete/%d" getDeleteAlbum

        url_regex "(.*?)\.(?!js$|css$|png$|gif$).*" >>= RequestErrors.FORBIDDEN "Access denied."
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