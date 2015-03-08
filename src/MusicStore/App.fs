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

open MusicStore.Domain
open MusicStore.Db
open MusicStore.View

let bindForm key = Binding.form key Choice1Of2

let albumForm req = binding {
        let! artistId = req |> bindForm "artist" >>. Parse.int32
        let! genreId = req |> bindForm "genre" >>. Parse.int32
        let! title = req |> bindForm "title"
        let! price = req |> bindForm "price" >>. Parse.decimal
        let! artUrl = req |> bindForm "artUrl"

        return artistId, genreId, title, price, artUrl
    }

let HTML vF getF (x: HttpContext) = async {
        let ctx = sql.GetDataContext()
        let genres = Db.getGenres ctx 
        let model = getF ctx
        
        let con = index genres (vF model) |> Html.xmlToString

        return! (OK con >>= Writers.setMimeType "text/html; charset=utf-8") x
    }

let backToManageStore postF (x: HttpContext) = async {
        let ctx = sql.GetDataContext()
        postF ctx
        return! Redirection.redirect "/store/manage" x   
    }

let store db = Db.getGenres db
let albumDetails id db = Db.getAlbum id db

let albumsForGenre name db = 
    let genre = Db.getGenre name db
    let albums = Db.getAlbumsForGenre genre.GenreId db
    genre.Name, albums

let manageStore db = Db.getAlbums db

let createAlbum db = Db.getGenres db, Db.getArtists db

let updateAlbum id db = Db.getAlbum id db, Db.getGenres db, Db.getArtists db 

let deleteAlbum id db = Db.getAlbum id db

choose [
    GET >>= choose [
        path "/" >>= (HTML vHome (fun _ -> ()))
        path "/store" >>= (HTML viewStore store)
        path "/store/browse" 
            >>= Binding.bindReq 
                    (Binding.query "genre" Choice1Of2) 
                    (albumsForGenre >> (HTML viewAlbumsForGenre))
                    BAD_REQUEST
        pathScan "/store/details/%d" (albumDetails >> HTML viewAlbumDetails)

        path "/store/manage" >>= (HTML vManageStore manageStore)
        path "/store/manage/create" >>= (HTML vCreateAlbum createAlbum)
        pathScan "/store/manage/edit/%d" (updateAlbum >> (HTML vEditAlbum))
        pathScan "/store/manage/delete/%d" (deleteAlbum >> (HTML vDeleteAlbum))

        pathRegex "(.*?)\.(?!js$|css$|png$|gif$).*" >>= RequestErrors.FORBIDDEN "Access denied."
        Files.browseHome
    ]

    POST >>= choose [
        path "/store/manage/create" 
            >>= (Binding.bindReq 
                    (albumForm >> Choice.map CreateAlbumCommand.create) 
                    (Db.createAlbum >> backToManageStore) 
                    BAD_REQUEST)
        pathScan "/store/manage/edit/%d" 
            (fun id -> 
                Binding.bindReq
                    (albumForm >> Choice.map (UpdateAlbumCommand.create id)) 
                    (Db.updateAlbum >> backToManageStore) 
                    BAD_REQUEST)
        pathScan "/store/manage/delete/%d" (Db.deleteAlbum >> backToManageStore)
    ]

    NOT_FOUND "404"
]
|> startWebServer {defaultConfig with bindings = [HttpBinding.mk' HTTP "127.0.0.1" 8028]}