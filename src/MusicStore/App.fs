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
open MusicStore.Form
open MusicStore.View

let passHash (pass: string) =
    use sha = Security.Cryptography.SHA256.Create()
    Text.Encoding.UTF8.GetBytes(pass)
    |> sha.ComputeHash
    |> Array.map (fun b -> b.ToString("x2"))
    |> String.concat ""

let redirectWithReturnPath redirection =
    request (fun x ->
        let path = x.url.AbsolutePath
        Redirection.FOUND (redirection |> Path.withParam ("returnPath", path)))

let returnPathOrHome = 
    request (fun x -> 
        let path = defaultArg (x.queryParam "returnPath") Path.home
        Redirection.FOUND path)

let lift success = function
    | Some x -> success x
    | None -> never

let withDb f = warbler (fun _ -> f (sql.GetDataContext()))

let overwriteCookiePathToRoot cookieName =
    context (fun x ->
        let cookie = x.response.cookies.[cookieName]
        let cookie' = (snd HttpCookie.path_) (Some Path.home) cookie
        setCookie cookie')

let bindForm form handler =
    Binding.bindReq (FormUtils.bindForm form) handler BAD_REQUEST

let bindQuery key handler =
    Binding.bindReq (Binding.query key Choice1Of2) handler BAD_REQUEST
    
type UserLoggedOnSession = {
    Username : string
    Role : string
}

type Session = 
    | NoSession
    | CartIdOnly of string
    | UserLoggedOn of UserLoggedOnSession

let session f = 
    statefulForSession
    >>= overwriteCookiePathToRoot State.CookieStateStore.StateCookie
    >>= context (fun x -> 
        match x |> HttpContext.state with
        | None -> f NoSession
        | Some state ->
            match state.get "cartid", state.get "username", state.get "role" with
            | Some cartId, None, None -> f (CartIdOnly cartId)
            | _, Some username, Some role -> f (UserLoggedOn {Username = username; Role = role})
            | _ -> f NoSession)

let reset =
    unsetPair Auth.SessionAuthCookie
    >>= unsetPair StateCookie
    >>= Redirection.FOUND Path.home

let loggedOn f_success =
    Auth.authenticate
        Cookie.CookieLife.Session
        false
        (fun () -> Choice2Of2(redirectWithReturnPath Path.Account.logon))
        (fun _ -> Choice2Of2 reset)
        f_success

let admin f_success = 
    Auth.authenticate
        Cookie.CookieLife.Session
        false
        (fun () -> Choice2Of2(UNAUTHORIZED "Not logged on"))
        (fun _ -> Choice2Of2 reset)
        (session (function
            | UserLoggedOn { Role = "admin" } -> f_success
            | UserLoggedOn _ -> FORBIDDEN "Only for admin"
            | _ -> UNAUTHORIZED "Not logged on" ))

let setSession setF = context (HttpContext.state >> lift setF)

let HTML container = 
    context (fun x ->
        let db = sql.GetDataContext()
        let result (itemsNumber, user) =
            let partUser = partUser (user |> Option.map (fun u -> u.Username))
            let partCategories = partCategories (Db.getGenres db)
            let partNav = partNav (user |> Option.map (fun u -> u.Role)) itemsNumber
            let content = viewIndex partUser partCategories partNav container |> Html.xmlToString
            OK content >>= Writers.setMimeType "text/html; charset=utf-8"    

        session(function
        | NoSession ->
            result (0, None)
        | CartIdOnly cartId ->
            result (Db.getCartsDetails cartId db |> List.sumBy (fun c -> c.Count), None)
        | UserLoggedOn user ->
            result (Db.getCartsDetails user.Username db |> List.sumBy (fun c -> c.Count), Some user))
        )

[<AutoOpen>]
module Handlers =
    open FormUtils

    let home = withDb (Db.getBestSellers >> viewHome >> HTML)
    
    let store = withDb (Db.getGenres >> viewStore >> HTML)

    let albumDetails id = withDb (Db.getAlbumDetails id >> lift (viewAlbumDetails >> HTML))

    let albumsForGenre name = withDb (Db.getAlbumsForGenre name >> viewAlbumsForGenre name >> HTML)

    let logon = viewLogon >> HTML

    let logoff = reset

    let register = viewRegister >> HTML

    let logonP (form : Logon) =
        let db = sql.GetDataContext()
        let (Password password) = form.Password
        match Db.validateUser(form.Username, passHash password) db with
        | Some user ->
                Auth.authenticated Cookie.CookieLife.Session false 
                >>= overwriteCookiePathToRoot Auth.SessionAuthCookie
                >>= session (function
                    | CartIdOnly cartId ->
                        let db = sql.GetDataContext()
                        Db.upgradeCarts (cartId, user.UserName) db
                        setSession (fun store -> store.set "cartid" "")
                    | _ -> succeed)
                >>= setSession (fun store ->
                    store.set "username" user.UserName
                    >>= store.set "role" user.Role)
                >>= returnPathOrHome
        | _ ->
            logon (Some "Username or password is invalid.")

    let registerP (form : Register) =
        let db = sql.GetDataContext()
        match Db.getUser form.Username db with
        | Some existing -> 
            register (Some "Sorry this username is already taken. Try another one.")
        | None ->
            let (Password password) = form.Password
            let (Email email) = form.Email
            Db.newUser (form.Username, passHash password, email) (sql.GetDataContext())
            logonP {Username = form.Username; Password = form.Password}
    
    let cart = 
        session (function
            | NoSession -> viewEmptyCart |> HTML
            | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
                withDb (Db.getCartsDetails cartId >> viewCart >> HTML))

    let addToCart albumId = 
        session (function
            | NoSession -> 
                let db = sql.GetDataContext()
                let cartId = Guid.NewGuid().ToString("N")
                Db.addToCart cartId albumId db
                setSession (fun state ->
                    state.set "cartid" cartId)
            | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
                let db = sql.GetDataContext()
                Db.addToCart cartId albumId db
                succeed)
        >>= Redirection.FOUND Path.Cart.overview

    let removeFromCart albumId =
        session (function
        | NoSession -> never
        | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
            let db = sql.GetDataContext()
            Db.getCart cartId albumId db 
            |> lift (fun cart -> 
                Db.removeFromCart cart albumId db
                db |> (Db.getCartsDetails cartId >> viewCart >> Html.flatten >> Html.xmlToString) |> OK))

    let checkout =
        session (function
        | NoSession | CartIdOnly _ -> never
        | UserLoggedOn _ -> viewCheckout |> HTML)
        
    let checkoutP form = 
        session (function
        | NoSession | CartIdOnly _ -> never
        | UserLoggedOn { Username = username } ->
            Db.placeOrder username (sql.GetDataContext())
            viewCheckoutComplete |> HTML)

    let manage = withDb (Db.getAlbumsDetails >> viewManageStore >> HTML)

    let createAlbum = 
        let getF db = Db.getGenres db, Db.getArtists db
        withDb (getF >> viewCreateAlbum >> HTML)
    
    let setAlbum (f : Album) = (fun (album : Db.Album) -> 
            album.ArtistId <- int f.ArtistId
            album.GenreId <- int f.GenreId
            album.Title <- f.Title
            album.Price <- f.Price
            album.AlbumArtUrl <- f.ArtUrl
        )

    let createAlbumP (form : Album) = 
        Db.newAlbum (int form.ArtistId, int form.GenreId, form.Price, form.Title) (sql.GetDataContext())
        Redirection.FOUND Path.Admin.manage

    let editAlbum id = 
        let getF db =
            match Db.getAlbum id db with
            | Some a -> Some (a, Db.getGenres db, Db.getArtists db)
            | None -> None 
        withDb (getF >> lift (viewEditAlbum >> HTML))
    
    let editAlbumP id form = 
        let db = sql.GetDataContext()
        Db.getAlbum id db
        |> lift (fun album ->
            Db.updateAlbum album (int form.ArtistId, int form.GenreId, form.Price, form.Title) db
            succeed
        )
        >>= Redirection.FOUND Path.Admin.manage

    let deleteAlbum id = 
        withDb (Db.getAlbumDetails id >> lift (viewDeleteAlbum >> HTML))
    
    let deleteAlbumP id =
        let db = sql.GetDataContext()
        Db.getAlbum id db
        |> lift (fun album ->
            Db.deleteAlbum album db
            succeed
        )
        >>= Redirection.FOUND Path.Admin.manage

choose [
    GET >>= choose [
        path Path.home >>= home
        path Path.Store.overview >>= store
        path Path.Store.browse >>= bindQuery Path.Store.browseKey albumsForGenre
        pathScan Path.Store.details albumDetails

        path Path.Account.logon >>= logon None
        path Path.Account.logoff >>= logoff
        path Path.Account.register >>= register None

        path Path.Cart.overview >>= cart
        pathScan Path.Cart.addAlbum addToCart
        
        path Path.Cart.checkout >>= loggedOn checkout

        path Path.Admin.manage >>= admin manage
        path Path.Admin.createAlbum >>= admin createAlbum
        pathScan Path.Admin.editAlbum (fun id -> admin (editAlbum id))
        pathScan Path.Admin.deleteAlbum (fun id -> admin (deleteAlbum id))

        pathRegex "(.*)\.(js|css|png|gif)" >>= Files.browseHome
    ]

    POST >>= choose [
        path Path.Account.logon >>= bindForm Form.logon logonP
        path Path.Account.register >>= bindForm Form.register registerP
        
        pathScan Path.Cart.removeAlbum removeFromCart
        
        path Path.Cart.checkout >>= loggedOn (bindForm Form.checkout checkoutP)
        
        path Path.Admin.createAlbum >>= admin (bindForm Form.album createAlbumP)
        pathScan Path.Admin.editAlbum (fun id -> admin (bindForm Form.album (editAlbumP id)))
        pathScan Path.Admin.deleteAlbum (fun id -> admin (deleteAlbumP id))
    ]

    HTML pageNotFound
]
|> startWebServer 
    { defaultConfig with 
        bindings = [ HttpBinding.mk' HTTP "127.0.0.1" 8028 ]
        logger = Logging.Loggers.ConsoleWindowLogger(Logging.Debug) }