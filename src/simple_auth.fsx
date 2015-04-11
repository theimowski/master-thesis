#r "C:/git/master-thesis/packages/Suave/lib/net40/Suave.dll"
#r "C:/git/master-thesis/packages/FsPickler/lib/net45/FsPickler.dll"

open Suave
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.Successful
open Suave.Http.RequestErrors
open Suave.State.CookieStateStore
open Suave.Web
open Suave.Types
open Suave.Cookie
open Suave.State

let reset =
    unsetPair Auth.SessionAuthCookie
    >>= unsetPair CookieStateStore.StateCookie
    >>= Redirection.redirect "/"

let app = 
    choose [ 
        path "/" >>= OK "root"
        path "/logon" 
            >>= Auth.authenticated Cookie.CookieLife.Session false 
            >>= statefulForSession
            >>= context (fun x -> 
                match x |> HttpContext.state with
                | Some store -> store.set "name" "tomek"
                | None -> Redirection.redirect "/")
            >>= Redirection.redirect "/loggedon" 
        path "/logoff" >>= reset
        path "/loggedon"
            >>= Auth.authenticate 
                    Cookie.CookieLife.Session 
                    false
                    (fun () -> Choice2Of2(BAD_REQUEST "no cookie"))
                    (fun _ -> Choice2Of2(reset))
                    (statefulForSession
                        >>= context (fun x -> 
                            match x |> HttpContext.state with
                            | Some store -> 
                                match store.get "name" with
                                | Some name -> sprintf "Hello %s" name |> OK
                                | None -> BAD_REQUEST "no name in store"
                            | None -> BAD_REQUEST "no state"))
        path "/state"
            >>= statefulForSession
            >>= context (fun x ->
                match x |> HttpContext.state with
                | Some store -> 
                    match store.get "name" with
                    | Some name -> OK ("State: " + name)
                    | None -> BAD_REQUEST "no name in store"
                | None -> BAD_REQUEST "no state")
    ]

startWebServer defaultConfig app
