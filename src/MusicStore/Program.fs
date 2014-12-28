open Suave                 // always open suave
open Suave.Http.Successful // for OK-result
open Suave.Web             // for config
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.RequestErrors

choose [
    GET >>= choose [
        url "/store" >>= (OK "Hello from store")
        url "/store/browse" >>= (OK "Hello from browse")
        url "/store/details" >>= (OK "Hello from details")
    ]

    NOT_FOUND "404"
]
|> web_server default_config 