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

let logonForm req =
    binding {
        let! username = req |> bindForm "username"
        let! password = req |> bindForm "password"

        return username,password
    }


let requireLogon =
    context (fun x ->
        let path = x.request.url.AbsolutePath
        Redirection.FOUND (sprintf "/account/logon?returnPath=%s" path))

let returnPathOrRoot = 
    request (fun x -> 
        let path = defaultArg (x.queryParam "returnPath") "/"
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

let tee f x = 
    f x
    succeed

let withDb f = warbler (fun _ -> f (sql.GetDataContext()))

let overwriteCookiePathToRoot cookieName =
    context (fun x ->
        let cookie = x.response.cookies.[cookieName]
        let cookie' = (snd HttpCookie.path_) (Some "/") cookie
        setCookie cookie')

let passHash (pass: string) =
    use sha = Security.Cryptography.SHA256.Create()
    Text.Encoding.UTF8.GetBytes(pass)
    |> sha.ComputeHash
    |> Array.map (fun b -> b.ToString("x2"))
    |> String.concat ""

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
        >>= Redirection.FOUND "/"

    let register = viewRegister |> HTML

    let logonP (username,password) =
        let auth db =
            match Db.validateUser (username, passHash password) db with
            | Some user ->
                    Auth.authenticated Cookie.CookieLife.Session false 
                    >>= sessionLogOnUser (username, user.Role)
                    >>= returnPathOrRoot
            | _ ->
                logon

        withDb auth

    let registerP (x: HttpContext) = async {
        let username = x.request |> bindForm "username"
        let email = x.request |> bindForm "email"
        let password = x.request |> bindForm "password"
        let confirmpassword = x.request |> bindForm "confirmpassword"
        match username, email, password with
        | Choice1Of2 username, Choice1Of2 email, Choice1Of2 password ->
            let createUser db =
                Db.newUser (email, passHash password, "user", username) db |> ignore
                db.SubmitUpdates()
                Redirection.FOUND "/account/logon"
            return! (withDb createUser) x
        | _ ->
            return! BAD_REQUEST "Missing username, email or password" x
    }
    
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
                Redirection.FOUND "/cart")

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
        
    let checkoutP = 
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
    
    let createAlbumP f = 
        sql.GetDataContext() |> (Db.newAlbum >> f)
        Redirection.FOUND "/store/manage"

    let editAlbum id = 
        let getF db =
            match Db.getAlbumDetails id db with
            | Some a -> Some (a, Db.getGenres db, Db.getArtists db)
            | None -> None 
        withDb (getF >> lift (viewEditAlbum >> HTML))
    
    let editAlbumP id f = 
        sql.GetDataContext() |> (Db.getAlbum id >> lift (tee f))
        >>= Redirection.FOUND "/store/manage"

    let deleteAlbum id = 
        withDb (Db.getAlbumDetails id >> lift (viewDeleteAlbum >> HTML >> admin))
    
    let deleteAlbumP id =
        sql.GetDataContext() |> (Db.getAlbum id >> lift (tee (fun a -> a.Delete())))
        >>= Redirection.FOUND "/store/manage"

choose [
    GET >>= choose [
        path "/" >>= home
        path "/store" >>= store
        path "/store/browse" 
            >>= Binding.bindReq (Binding.query "genre" Choice1Of2) albumsForGenre BAD_REQUEST
        pathScan "/store/details/%d" albumDetails

        path "/account/logon" >>= logon
        path "/account/logoff" >>= logoff
        path "/account/register" >>= register

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
        path "/account/logon" 
            >>= (Binding.bindReq logonForm logonP BAD_REQUEST)
        path "/account/register" >>= registerP
        
        pathScan "/cart/remove/%d" removeFromCart
        path "/cart/checkout" >>= checkoutP

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