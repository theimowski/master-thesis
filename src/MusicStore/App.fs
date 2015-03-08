module MusicStore.App

open System

open Suave
open Suave.Http.Successful
open Suave.Web
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.RequestErrors
open Suave.Types
open Suave.Model
open Suave.Utils

open MusicStore.Db
open MusicStore.View

let bindForm key = Binding.form key Choice1Of2

let albumForm req = binding {
        let! artistId = req |> bindForm "artist" >>. Parse.int32
        let! genreId = req |> bindForm "genre" >>. Parse.int32
        let! title = req |> bindForm "title"
        let! price = req |> bindForm "price" >>. Parse.decimal
        let! artUrl = req |> bindForm "artUrl"

        return (fun (album : Db.Album) -> 
            album.ArtistId <- artistId
            album.GenreId <- genreId
            album.Title <- title
            album.Price <- price
            album.AlbumArtUrl <- artUrl
        )
    }

let HTML viewF getF (x: HttpContext) = async {
        let ctx = sql.GetDataContext()
        let genres = Db.getGenres ctx 
        let model = getF ctx
        
        let con = viewIndex genres (viewF model) |> Html.xmlToString

        return! (OK con >>= Writers.setMimeType "text/html; charset=utf-8") x
    }

let backToManageStore postF (x: HttpContext) = async {
        let ctx = sql.GetDataContext()
        postF ctx
        return! Redirection.redirect "/store/manage" x   
    }

let home = HTML viewHome (fun _ -> ())

let store = HTML viewStore Db.getGenres

let albumDetails id = HTML viewAlbumDetails (Db.getAlbumDetails id)

let albumsForGenre name db = 
    let genre = Db.getGenre name db
    let albums = Db.getAlbumsForGenre genre.GenreId db
    genre.Name, albums

let manage = HTML viewManageStore Db.getAlbumsDetails

let createAlbum = HTML viewCreateAlbum (fun db -> Db.getGenres db, Db.getArtists db)

let editAlbum id = HTML viewEditAlbum (fun db -> Db.getAlbumDetails id db, Db.getGenres db, Db.getArtists db)

let deleteAlbum id = HTML viewDeleteAlbum (Db.getAlbumDetails id)

choose [
    GET >>= choose [
        path "/" >>= home
        path "/store" >>= store
        path "/store/browse" 
            >>= Binding.bindReq 
                    (Binding.query "genre" Choice1Of2) 
                    (albumsForGenre >> (HTML viewAlbumsForGenre))
                    BAD_REQUEST
        pathScan "/store/details/%d" albumDetails

        path "/store/manage" >>= manage
        path "/store/manage/create" >>= createAlbum
        pathScan "/store/manage/edit/%d" editAlbum
        pathScan "/store/manage/delete/%d" deleteAlbum

        pathRegex "(.*?)\.(?!js$|css$|png$|gif$).*" >>= RequestErrors.FORBIDDEN "Access denied."
        Files.browseHome
    ]

    POST >>= choose [
        path "/store/manage/create"
            >>= (Binding.bindReq
                    albumForm
                    (Db.saveAlbum Db.newAlbum >> backToManageStore)
                    BAD_REQUEST)
        pathScan "/store/manage/edit/%d" 
            (fun id -> 
                Binding.bindReq
                    albumForm 
                    (Db.saveAlbum (Db.getAlbum id) >> backToManageStore) 
                    BAD_REQUEST)
        pathScan "/store/manage/delete/%d" (Db.deleteAlbum >> backToManageStore)
    ]

    NOT_FOUND "404"
]
|> startWebServer {defaultConfig with bindings = [HttpBinding.mk' HTTP "127.0.0.1" 8028]}