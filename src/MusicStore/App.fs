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

let requireLogon =
    context (fun x ->
        let path = x.request.url.AbsolutePath
        Redirection.FOUND (Path.Account.logon |> Path.withParam ("returnPath", path)))

let returnPathOrHome = 
    request (fun x -> 
        let path = defaultArg (x.queryParam "returnPath") Path.home
        Redirection.FOUND path)

let lift success = function
    | Some x -> success x
    | None -> never

let withDb f = warbler (fun _ -> f (sql.GetDataContext()))

//let overwriteCookiePathToRoot cookieName =
//    context (fun x ->
//        let cookie = x.response.cookies.[cookieName]
//        let cookie' = (snd HttpCookie.path_) (Some Path.home) cookie
//        setCookie cookie')


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
        (fun () -> Choice2Of2(requireLogon))
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
            let partUser = partUser (user |> Option.map fst)
            let partCategories = partCategories (Db.getGenres db)
            let partNav = partNav (user |> Option.map snd) (itemsNumber)
            let content = viewIndex partUser partCategories partNav container |> Html.xmlToString
            OK content >>= Writers.setMimeType "text/html; charset=utf-8"    

        session(function
        | NoSession ->
            result (0, None)
        | CartIdOnly cartId ->
            result (Db.getCartsDetails cartId db |> List.sumBy (fun c -> c.Count), None)
        | UserLoggedOn {Username = username; Role = role} ->
            result (Db.getCartsDetails username db |> List.sumBy (fun c -> c.Count), Some (username, role)))
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

    let logonP ({ Password = Password p } as f : Logon) =
        let db = sql.GetDataContext()
        match Db.validateUser(f.Username, passHash p) db with
        | Some user ->
                Auth.authenticated Cookie.CookieLife.Session false 
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

    let registerP (f : Register) =
        let set = (fun (user : User) ->
                user.UserName <- f.Username
                user.Email <- (let (Email email) = f.Email in email)
                user.Password <- (let (Password password) = f.Password in passHash password)
                user.Role <- "user"
            )   
        let db = sql.GetDataContext()
        match Db.getUser f.Username db with
        | Some existing -> 
            register (Some "Sorry this username is already taken. Try another.")
        | None ->
            Db.newUser set (sql.GetDataContext())
            Redirection.redirect Path.Account.logon
    
    let cart = 
        session (function
            | NoSession -> viewCart [] |> HTML
            | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
                withDb (Db.getCartsDetails cartId >> viewCart >> HTML))

    let addToCart albumId = 
        session (function
            | NoSession -> 
                setSession (fun state ->
                    let db = sql.GetDataContext()
                    let cartId = Guid.NewGuid().ToString("N")
                    Db.addToCart cartId albumId db
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
        
    let checkoutP f = 
        session (function
        | NoSession | CartIdOnly _ -> never
        | UserLoggedOn { Username = username } ->
            let order = Db.placeOrder username (sql.GetDataContext())
            viewCheckoutComplete order.OrderId |> HTML)

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

    let createAlbumP result = 
        Db.newAlbum (setAlbum result) (sql.GetDataContext())
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
            setAlbum form album
            db.SubmitUpdates()
            succeed
        )
        >>= Redirection.FOUND Path.Admin.manage

    let deleteAlbum id = 
        withDb (Db.getAlbumDetails id >> lift (viewDeleteAlbum >> HTML))
    
    let deleteAlbumP id =
        let db = sql.GetDataContext()
        Db.getAlbum id db
        |> lift (fun album ->
            album.Delete()
            db.SubmitUpdates()
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

    NOT_FOUND "404"
]
|> startWebServer 
    { defaultConfig with 
        bindings = [ HttpBinding.mk' HTTP "127.0.0.1" 8028 ]
        logger = Logging.Loggers.ConsoleWindowLogger(Logging.Debug) }