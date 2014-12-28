open Suave                 // always open suave
open Suave.Http.Successful // for OK-result
open Suave.Web             // for config
open Suave.Http
open Suave.Http.Applicatives

choose [
    GET >>= choose [
        url "/store" >>= (OK "Hello from store")
        url "/store/browse" >>= (OK "Hello from browse")
        url "/store/details" >>= (OK "Hello from details")
    ]
]
|> web_server default_config 