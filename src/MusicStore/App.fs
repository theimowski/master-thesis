﻿module MusicStore.App

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
        Redirection.FOUND (sprintf "/account/logon?returnPath=%s" path))

let returnPathOrRoot = 
    request (fun x -> 
        let path = defaultArg (x.queryParam "returnPath") Path.home
        Redirection.FOUND path)

let loggedOn f_success =
    context (fun x  -> 
        let path = x.request.url.AbsolutePath
        Auth.authenticate
            Cookie.CookieLife.Session
            false
            (fun () -> Choice2Of2(requireLogon))
            (sprintf "%A" >> RequestErrors.BAD_REQUEST >> Choice2Of2)
            f_success)


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

let clearUserState = 
    Writers.unsetUserData StateStoreType

type UserLoggedOnSession = {
    Username : string
    Role : string
}

type Session = 
    | NoSession
    | CartIdOnly of string
    | UserLoggedOn of UserLoggedOnSession

let getSession (x: HttpContext) =
    match x |> HttpContext.state with
    | None -> NoSession
    | Some store ->
        match store.get "cartid", store.get "username", store.get "role" with
        | Some cartId, None, None -> CartIdOnly cartId
        | None, Some username, Some role -> UserLoggedOn {Username = username; Role = role}
        | _ -> NoSession

let session f = 
    context (fun x -> f (getSession x))

let sessionSetCartId =
    session (function
    | NoSession -> 
        statefulForSession
        >>= context (HttpContext.state >> lift (fun store -> 
            store.set "cartid" (Guid.NewGuid().ToString("N"))))
    | _ -> succeed)

let sessionLogOnUser  (username,role) =
    session (function
    | NoSession ->
        statefulForSession
        >>= context (HttpContext.state >> lift (fun store ->
            store.set "username" username
            >>= store.set "role" role))
    | CartIdOnly cartId ->
        let upgradeCartId db = 
            for cart in Db.getCarts cartId db do
                cart.CartId <- username
            db.SubmitUpdates()
            succeed
        context (HttpContext.state >> lift (fun store -> 
            clearUserState
            >>= store.set "username" username
            >>= store.set "role" role
            >>= withDb upgradeCartId))
    | UserLoggedOn _ -> never)

let sessionLogOffUser = 
    session (function
    | NoSession | CartIdOnly _ -> never
    | UserLoggedOn _ -> clearUserState)

let role (r : string) f_success = 
    session (function
    | NoSession | CartIdOnly _ -> requireLogon
    | UserLoggedOn { Role = role } ->
        if role = r 
        then f_success
        else FORBIDDEN (sprintf "Only for %s role" r))
    
let admin = loggedOn >> role "admin"

let HTML html = 
    context (fun x ->
        let db = sql.GetDataContext()
        let genres = Db.getGenres db
        let cartItems, username = 
            match getSession x with
            | NoSession -> 0, None
            | CartIdOnly cartId ->
                Db.getCartsDetails cartId db |> List.sumBy (fun c -> c.Count), None
            | UserLoggedOn {Username = username} ->
                Db.getCartsDetails username db |> List.sumBy (fun c -> c.Count), Some username

        let content = viewIndex (genres, cartItems, username) html |> Html.xmlToString

        OK content >>= Writers.setMimeType "text/html; charset=utf-8")

[<AutoOpen>]
module Handlers =
    open FormUtils

    let home = withDb (Db.getBestSellers >> viewHome >> HTML)
    
    let store = withDb (Db.getGenres >> viewStore >> HTML)

    let albumDetails id = withDb (Db.getAlbumDetails id >> lift (viewAlbumDetails >> HTML))

    let albumsForGenre name = 
        let getF db =
            Db.getGenre name db 
            |> Option.map (fun genre -> Db.getAlbumsForGenre genre.GenreId db)
        withDb (getF >> lift (viewAlbumsForGenre name >> HTML))

    let logon = viewLogon |> HTML

    let logoff =
        sessionLogOffUser
        >>= unsetPair Auth.SessionAuthCookie
        >>= unsetPair StateCookie
        >>= Redirection.FOUND Path.home
        |> loggedOn

    let register = viewRegister |> HTML

    let logonP (f : Logon) =
        let auth db =
            match Db.validateUser 
                    (f.Username, 
                     passHash (f.Password)) db with
            | Some user ->
                    Auth.authenticated Cookie.CookieLife.Session false 
                    >>= sessionLogOnUser (f.Username, user.Role)
                    >>= returnPathOrRoot
            | _ ->
                logon

        withDb auth

    let registerP (f : Register) =
        let set = (fun (user : User) ->
                user.UserName <- f.Username
                user.Email <- match f.Email with | Some (Email e) -> e | None -> ""
                user.Password <- passHash f.Password
                user.Role <- "user"
            )   
        Db.newUser set (sql.GetDataContext())
        Redirection.FOUND Path.Account.logon
    
    let cart = 
        session (function
            | NoSession -> viewCart [] |> HTML
            | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
                withDb (Db.getCartsDetails cartId >> viewCart >> HTML))

    let addToCart albumId = 
        sessionSetCartId
        >>= session (function
            | NoSession -> never
            | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
                Db.addToCart cartId albumId (sql.GetDataContext())
                Redirection.FOUND Path.Cart.overview)

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
        |> loggedOn 
        
    let checkoutP f = 
        session (function
        | NoSession | CartIdOnly _ -> never
        | UserLoggedOn { Username = username } ->
            let order = Db.placeOrder username (sql.GetDataContext())
            viewCheckoutComplete order.OrderId |> HTML)
        |> loggedOn

    let manage = withDb (Db.getAlbumsDetails >> viewManageStore >> HTML >> admin)

    let createAlbum = 
        let getF db = Db.getGenres db, Db.getArtists db
        withDb (getF >> viewCreateAlbum >> HTML >> admin)
    
    let setAlbum (f : Album) = (fun (album : Db.Album) -> 
            album.ArtistId <- f.ArtistId
            album.GenreId <- f.GenreId
            album.Title <- f.Title
            album.Price <- f.Price
            album.AlbumArtUrl <- f.ArtUrl
        )

    let createAlbumP result = 
        sql.GetDataContext() |> Db.newAlbum (setAlbum result)
        Redirection.FOUND Path.Admin.manage

    let editAlbum id = 
        let getF db =
            match Db.getAlbum id db with
            | Some a -> Some (a, Db.getGenres db, Db.getArtists db)
            | None -> None 
        withDb (getF >> lift (viewEditAlbum >> HTML))
    
    let editAlbumP id result = 
        sql.GetDataContext() |> (fun db -> Db.getAlbum id db |> lift (fun a -> (setAlbum result) a; db.SubmitUpdates(); succeed))
        >>= Redirection.FOUND Path.Admin.manage

    let deleteAlbum id = 
        withDb (Db.getAlbumDetails id >> lift (viewDeleteAlbum >> HTML >> admin))
    
    let deleteAlbumP id =
        sql.GetDataContext() |> (fun db -> Db.getAlbum id db |> lift (fun a -> a.Delete(); db.SubmitUpdates(); succeed))
        >>= Redirection.FOUND Path.Admin.manage

choose [
    GET >>= choose [
        path Path.home >>= home
        path Path.Store.overview >>= store
        path Path.Store.browse >>= bindQuery Path.Store.browseKey albumsForGenre
        pathScan Path.Store.details albumDetails

        path Path.Account.logon >>= logon
        path Path.Account.logoff >>= logoff
        path Path.Account.register >>= register

        path Path.Cart.overview >>= cart
        pathScan Path.Cart.addAlbum addToCart
        path Path.Cart.checkout >>= checkout

        path Path.Admin.manage >>= manage
        path Path.Admin.createAlbum >>= createAlbum
        pathScan Path.Admin.editAlbum editAlbum
        pathScan Path.Admin.deleteAlbum deleteAlbum

        path "/jquery-1.11.2.js" >>= Files.browseFileHome "jquery-1.11.2.js"
        pathRegex "(.*?)\.(?!js|css|png|gif).*" >>= RequestErrors.FORBIDDEN "Access denied."
        Files.browseHome
    ]

    POST >>= choose [
        path Path.Account.logon >>= bindForm Form.logon logonP
        path Path.Account.register >>= bindForm Form.register registerP
        
        pathScan Path.Cart.removeAlbum removeFromCart
        path Path.Cart.checkout >>= bindForm Form.checkout checkoutP

        path Path.Admin.createAlbum >>= bindForm Form.album createAlbumP
        pathScan Path.Admin.editAlbum 
            (fun id -> admin (bindForm Form.album (editAlbumP id)))
        pathScan Path.Admin.deleteAlbum 
            (fun id -> admin (deleteAlbumP id))
    ]

    NOT_FOUND "404"
]
|> startWebServer 
    { defaultConfig with 
        bindings = [ HttpBinding.mk' HTTP "127.0.0.1" 8028 ]
        logger = Logging.Loggers.ConsoleWindowLogger(Logging.Debug) }