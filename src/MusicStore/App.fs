module MusicStore.App

open System

open Suave
open Suave.Http.Successful
open Suave.Web
open Suave.Html
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

let HTML getF (x: HttpContext) = async {
        let ctx = sql.GetDataContext()
        let genres = Db.getGenres ctx
        let model = getF ctx
        
        let container = View.render model
        let index = {Index.Container = container; Genres = genres}
        return! (OK (View.render index) >>= Writers.setMimeType "text/html; charset=utf-8") x
    }

let HTMLR getF (x: HttpContext) = async {
        let ctx = sql.GetDataContext()
        let genres = Db.getGenres ctx
        let model : AlbumDetails = getF ctx
        
        let con =
            html [ 
                head [
                  title "F# Suave Music Store"
                  linkAttr [ "href", "/Site.css"; "rel", "stylesheet"; "type", "text/css" ]
                ] 
                body [
                  divAttr ["id", "header"] [
                    tag "h1" [] (tag "a" ["href", "/"] (text "F# Suave Music Store"))
                    tag "ul" ["id", "navlist"] 
                        (flatten [
                            tag "li" ["class", "first"] (tag "a" ["id", "current"; "href", "/"] (text "Home"))
                            tag "li" [] (tag "a" ["href", "/store"] (text "Store"))
                            tag "li" [] (tag "a" ["href", "/store/manage"] (text "Admin"))
                        ])
                  ]

                  divAttr ["id", "container"] [
                      tag "h2" [] (text model.Album.Title)
                      p [ imgAttr [ "src", "/placeholder.gif"] ]
                      divAttr ["id", "album-details"] [
                        p [
                            tag "em" [] (text "Genre:")
                            text model.Album.Genre
                        ]
                        p [
                            tag "em" [] (text "Artist:")
                            text model.Album.Artist
                        ]
                        p [
                            tag "em" [] (text "Price:")
                            text model.Album.Price
                        ]
                      ]
                  ]

                  divAttr ["id", "footer"] [
                    text "built with "
                    tag "a" ["href", "http://fsharp.org"] (text "F#")
                    text " and "
                    tag "a" ["href", "http://suave.io"] (text "Suave.IO")
                  ]

                ]
             ]
             |> xmlToString


        return! (OK con >>= Writers.setMimeType "text/html; charset=utf-8") x
    }

let backToManageStore postF (x: HttpContext) = async {
        let ctx = sql.GetDataContext()
        postF ctx
        return! Redirection.redirect "/store/manage" x   
    }

let home _ = { Placeholder = () }
let store db = { Store.Genres = Db.getGenres db }

let albumsForGenre name db = 
    let genre = Db.getGenre name db
    { AlbumsForGenre.Genre = genre
      Albums = Db.getAlbumsForGenre genre.Id db }

let albumDetails id db = { AlbumDetails.Album = Db.getAlbum id db }
let manageStore db = { ManageStore.Albums = Db.getAlbums db }

let createAlbum db = 
    { CreateAlbum.Artists = Db.getArtists db
      Genres = Db.getGenres db }

let updateAlbum id db = 
    { EditAlbum.Album = Db.getAlbum id db
      Artists = Db.getArtists db
      Genres = Db.getGenres db }

let deleteAlbum id db = { DeleteAlbum.Album = Db.getAlbum id db }

choose [
    GET >>= choose [
        path "/" >>= (HTML home)
        path "/store" >>= (HTML store)
        path "/store/browse" 
            >>= Binding.bindReq 
                    (Binding.query "genre" Choice1Of2) 
                    (albumsForGenre >> HTML)
                    BAD_REQUEST
        pathScan "/store/details/%d" (albumDetails >> HTMLR)

        path "/store/manage" >>= (HTML manageStore)
        path "/store/manage/create" >>= (HTML createAlbum)
        pathScan "/store/manage/edit/%d" (updateAlbum >> HTML)
        pathScan "/store/manage/delete/%d" (deleteAlbum >> HTML)

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