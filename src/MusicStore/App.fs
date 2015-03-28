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
open Suave.Cookie

open MusicStore.Db
open MusicStore.View

let bindForm key = Binding.form key Choice1Of2

let albumForm req = 
    
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

let HTML html (x: HttpContext) = async {
        let ctx = sql.GetDataContext()
        let genres = Db.getGenres ctx
        
        let content = viewIndex genres html |> Html.xmlToString

        return! (OK content >>= Writers.setMimeType "text/html; charset=utf-8") x
    }

let admin f_success (x: HttpContext) = async { 
        let path = x.request.url.AbsolutePath

        return! (Auth.authenticateWithLogin
                    Cookie.CookieLife.Session
                    (sprintf "/account/logon?returnPath=%s" path)
                    f_success) x
    }

let lift success = function
    | Some x -> success x
    | None -> (fun x -> fail)

let withCtx dbF = dbF (sql.GetDataContext())

[<AutoOpen>]
module Handlers =

    let home = viewHome |> HTML
    
    let store = withCtx Db.getGenres |> (viewStore >> HTML)

    let albumDetails = withCtx Db.getAlbumDetails >> lift (viewAlbumDetails >> HTML)

    let albumsForGenre = 
        let getF db name =
            match Db.getGenre name db with
            | Some genre ->
                let albums = Db.getAlbumsForGenre genre.GenreId db
                Some (genre,albums)
            | None -> None
        withCtx getF >> lift (viewAlbumsForGenre >> HTML)

    let logon = viewLogon |> HTML

    let logonP (x: HttpContext) = async {
        let username = x.request |> bindForm "username"
        let password = x.request |> bindForm "password"
        match username,password with
        | Choice1Of2 "admin", Choice1Of2 "admin" ->
            return! (
                Auth.authenticated Cookie.CookieLife.Session false 
                >>= context (fun x -> 
                    let cookie = x.response.cookies.[Auth.SessionAuthCookie]
                    let cookie' = (snd HttpCookie.path_) (Some "/") cookie
                    setCookie cookie'
                    )
                >>= request (fun x -> 
                    let path = defaultArg (x.queryParam "returnPath") "/"
                    Redirection.redirect path))  x
        | Choice1Of2 _, Choice1Of2 _ ->
            return! logon x
        | _ ->
            return! BAD_REQUEST "Missing username or password" x
    }

    let manage = withCtx Db.getAlbumsDetails |> (viewManageStore >> HTML >> admin)

    let createAlbum = 
        let getF db = Db.getGenres db, Db.getArtists db
        withCtx getF |> (viewCreateAlbum >> HTML >> admin)
    
    let createAlbumP f = 
        withCtx Db.saveAlbum ((fun () -> withCtx Db.newAlbum)) f 
        Redirection.redirect "/store/manage" 

    let editAlbum = 
        let getF db id =
            match Db.getAlbumDetails db id with
            | Some a -> Some (a, Db.getGenres db, Db.getArtists db)
            | None -> None 
        withCtx getF >> (lift (viewEditAlbum >> HTML))
    
    let editAlbumP id f = 
        match (withCtx Db.getAlbum) id with
        | Some album -> 
            withCtx Db.saveAlbum (fun () -> album) f
            Redirection.redirect "/store/manage" 
        | None -> fun x -> fail

    let deleteAlbum = 
        withCtx Db.getAlbumDetails >> lift (viewDeleteAlbum >> HTML >> admin)
    
    let deleteAlbumP = 
        withCtx Db.deleteAlbum >> lift (fun _ -> Redirection.redirect "/store/manage")


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
        path "/account/logon" >>= logonP

        path "/store/manage/create"
            >>= admin (Binding.bindReq albumForm createAlbumP BAD_REQUEST)
        pathScan "/store/manage/edit/%d" 
            (fun id -> admin (Binding.bindReq albumForm (editAlbumP id) BAD_REQUEST))
        pathScan "/store/manage/delete/%d" 
            (fun id -> admin (deleteAlbumP id))
    ]

    NOT_FOUND "404"
]
|> startWebServer 
    { defaultConfig with 
        bindings = [ HttpBinding.mk' HTTP "127.0.0.1" 8028 ]
        logger = Logging.Loggers.ConsoleWindowLogger(Logging.Debug) }