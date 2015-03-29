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
open Suave.State.CookieStateStore

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

let actOnAlbumAndBackToManage f (ctx : DbContext) album =
    f album
    ctx.SubmitUpdates()
    Redirection.redirect "/store/manage"

let lift success = function
    | Some x -> success x
    | None -> (fun _ -> fail)

let withDb f = warbler (fun _ -> f (sql.GetDataContext()))

let overwriteCookiePathToRoot cookieName =
    context (fun x ->
        let cookie = x.response.cookies.[cookieName]
        let cookie' = (snd HttpCookie.path_) (Some "/") cookie
        setCookie cookie')

[<AutoOpen>]
module Handlers =

    let home = viewHome |> HTML
    
    let store = withDb (Db.getGenres >> viewStore >> HTML)

    let albumDetails id = withDb (Db.getAlbumDetails id >> lift (viewAlbumDetails >> HTML))

    let albumsForGenre name = 
        let getF db =
            match Db.getGenre name db with
            | Some genre ->
                let albums = Db.getAlbumsForGenre genre.GenreId db
                Some (genre,albums)
            | None -> None
        withDb (getF >> lift (viewAlbumsForGenre >> HTML))

    let logon = viewLogon |> HTML

    let logonP (x: HttpContext) = async {
        let username = x.request |> bindForm "username"
        let password = x.request |> bindForm "password"
        match username,password with
        | Choice1Of2 "admin", Choice1Of2 "admin" ->
            return! (
                Auth.authenticated Cookie.CookieLife.Session false 
                >>= overwriteCookiePathToRoot Auth.SessionAuthCookie
                >>= request (fun x -> 
                    let path = defaultArg (x.queryParam "returnPath") "/"
                    Redirection.redirect path))  x
        | Choice1Of2 _, Choice1Of2 _ ->
            return! logon x
        | _ ->
            return! BAD_REQUEST "Missing username or password" x
    }

    let cart = 
        context (fun x ->
            match x |> HttpContext.state with
            | None -> 
                viewCart [] |> HTML
            | Some store -> 
                match store.get "cartId" with
                | Some cartId ->
                    withDb (Db.getCartsDetails cartId >> viewCart >> HTML)
                | None ->
                    viewCart [] |> HTML)

    let addToCart albumId = 
        let add cartId db  =
            match Db.getCart cartId albumId db with
            | Some cart ->
                cart.Count <- cart.Count + 1
            | None ->
                Db.newCart cartId albumId db |> ignore
            db.SubmitUpdates()
            Redirection.redirect "/cart"

        statefulForSession
        >>= context (fun x ->
            match x |> HttpContext.state with
            | None -> 
                Redirection.FOUND (sprintf "/cart/add/%d" albumId)
            | Some store -> 
                match store.get "cartId" with
                | Some cartId ->
                    withDb (add cartId)
                | None ->
                    let cartId = Guid.NewGuid().ToString("N")
                    store.set "cartId" cartId
                    >>= withDb (add cartId))
        >>= overwriteCookiePathToRoot StateCookie

    let removeFromCart albumId =
        let remove cartId db =
            match Db.getCart cartId albumId db with
            | Some cart ->
                cart.Count <- cart.Count - 1
                if cart.Count = 0 then cart.Delete()
                db.SubmitUpdates()
                (Db.getCartsDetails cartId >> viewCart >> Html.flatten >> Html.xmlToString) (sql.GetDataContext()) |> OK
            | None ->
                fun x -> fail
        
        context (fun x ->
            match x |> HttpContext.state with
            | None -> 
                fun x -> fail
            | Some store -> 
                match store.get "cartId" with
                | None ->
                    fun x -> fail
                | Some cartId ->
                    withDb (remove cartId)
                )

    let checkout =
        context (fun x ->
            match x |> HttpContext.state with
            | None -> 
                fun x -> fail
            | Some store -> 
                match store.get "cartId" with
                | None ->
                    fun x -> fail
                | Some cartId ->
                    viewCheckout |> HTML
                )

    let checkoutComplete = 
        context (fun x ->
            match x |> HttpContext.state with
            | None -> 
                fun x -> fail
            | Some store -> 
                match store.get "cartId" with
                | None ->
                    fun x -> fail
                | Some cartId ->
                    viewCheckoutComplete 28 |> HTML
                )

    let manage = withDb (Db.getAlbumsDetails >> viewManageStore >> HTML >> admin)

    let createAlbum = 
        let getF db = Db.getGenres db, Db.getArtists db
        withDb (getF >> viewCreateAlbum >> HTML >> admin)
    
    let createAlbumP f = 
        withDb (fun db -> 
                Db.newAlbum db 
                |> actOnAlbumAndBackToManage f db)

    let editAlbum id = 
        let getF db =
            match Db.getAlbumDetails id db with
            | Some a -> Some (a, Db.getGenres db, Db.getArtists db)
            | None -> None 
        withDb (getF >> lift (viewEditAlbum >> HTML))
    
    let editAlbumP id f = 
        withDb (fun db ->
                Db.getAlbum id db
                |> lift (actOnAlbumAndBackToManage f db))

    let deleteAlbum id = 
        withDb (Db.getAlbumDetails id >> lift (viewDeleteAlbum >> HTML >> admin))
    
    let deleteAlbumP id =
        let f (album : Album) = album.Delete()
        withDb (fun db ->
                Db.getAlbum id db
                |> lift (actOnAlbumAndBackToManage f db))

choose [
    GET >>= choose [
        path "/" >>= home
        path "/store" >>= store
        path "/store/browse" 
            >>= Binding.bindReq (Binding.query "genre" Choice1Of2) albumsForGenre BAD_REQUEST
        pathScan "/store/details/%d" albumDetails

        path "/account/logon" >>= logon

        path "/cart" >>= cart
        pathScan "/cart/add/%d" addToCart
        path "/cart/checkout" >>= checkout

        path "/store/manage" >>= manage
        path "/store/manage/create" >>= createAlbum
        pathScan "/store/manage/edit/%d" editAlbum
        pathScan "/store/manage/delete/%d" deleteAlbum

        path "/jquery-1.11.2.js" >>= Files.browseFileHome "jquery-1.11.2.js"
        pathRegex "(.*?)\.(?!js|css|png|gif).*" >>= RequestErrors.FORBIDDEN "Access denied."
        Files.browseHome
    ]

    POST >>= choose [
        path "/account/logon" >>= logonP
        
        pathScan "/cart/remove/%d" removeFromCart
        path "/cart/checkout" >>= checkoutComplete

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