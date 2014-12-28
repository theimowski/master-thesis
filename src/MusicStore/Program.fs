open Suave                 // always open suave
open Suave.Http.Successful // for OK-result
open Suave.Web             // for config
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.RequestErrors
open Suave.Types

choose [
    GET >>= choose [
        url "/store" >>= (OK "Hello from store")
        url "/store/browse" 
            >>= request(fun request -> cond (HttpRequest.query(request) ^^ "genre") 
                                            (fun genre -> OK (sprintf "Genre: %s" genre)) 
                                            never)
        url "/store/details" >>= (OK "Hello from details")
    ]

    NOT_FOUND "404"
]
|> web_server default_config 