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

let albumForm req = 
    let bindForm key = Binding.form key Choice1Of2
    
    binding {
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

let HTML getF (x: HttpContext) = async {
        let ctx = sql.GetDataContext()
        let genres = Db.getGenres ctx 
        
        let content = viewIndex genres (getF ctx) |> Html.xmlToString

        return! (OK content >>= Writers.setMimeType "text/html; charset=utf-8") x
    }

let postAndRedirectToManage postF (x: HttpContext) = async {
        let ctx = sql.GetDataContext()
        postF ctx
        return! Redirection.redirect "/store/manage" x   
    }

[<AutoOpen>]
module Handlers =

    let home = ignore >> viewHome |> HTML
    
    let store = Db.getGenres >> viewStore |> HTML

    let albumDetails id = Db.getAlbumDetails id >> viewAlbumDetails |> HTML

    let albumsForGenre name = 
        let getF db =
            let genre = Db.getGenre name db
            let albums = Db.getAlbumsForGenre genre.GenreId db
            genre,albums
        getF >> viewAlbumsForGenre |> HTML

    let logon = ignore >> viewLogon |> HTML

    let manage = Db.getAlbumsDetails >> viewManageStore |> HTML

    let createAlbum = 
        let getF db = Db.getGenres db, Db.getArtists db
        getF >> viewCreateAlbum |> HTML
    
    let createAlbumP = 
        Db.saveAlbum Db.newAlbum >> postAndRedirectToManage

    let editAlbum id = 
        let getF db = Db.getAlbumDetails id db, Db.getGenres db, Db.getArtists db
        getF >> viewEditAlbum |> HTML
    
    let editAlbumP id = 
        Db.saveAlbum (Db.getAlbum id) >> postAndRedirectToManage

    let deleteAlbum id = 
        Db.getAlbumDetails id >> viewDeleteAlbum |> HTML
    
    let deleteAlbumP = 
        Db.deleteAlbum >> postAndRedirectToManage

choose [
    GET >>= choose [
        path "/" >>= home
        path "/store" >>= store
        path "/store/browse" 
            >>= Binding.bindReq (Binding.query "genre" Choice1Of2) albumsForGenre BAD_REQUEST
        pathScan "/store/details/%d" albumDetails

        path "/account/logon" >>= logon

        path "/store/manage" >>= manage
        path "/store/manage/create" >>= createAlbum
        pathScan "/store/manage/edit/%d" editAlbum
        pathScan "/store/manage/delete/%d" deleteAlbum

        pathRegex "(.*?)\.(?!js$|css$|png$|gif$).*" >>= RequestErrors.FORBIDDEN "Access denied."
        Files.browseHome
    ]

    POST >>= choose [
        path "/store/manage/create"
            >>= Binding.bindReq albumForm createAlbumP BAD_REQUEST
        pathScan "/store/manage/edit/%d" 
            (fun id -> Binding.bindReq albumForm (editAlbumP id) BAD_REQUEST)
        pathScan "/store/manage/delete/%d" deleteAlbumP
    ]

    NOT_FOUND "404"
]
|> startWebServer {defaultConfig with bindings = [HttpBinding.mk' HTTP "127.0.0.1" 8028]}