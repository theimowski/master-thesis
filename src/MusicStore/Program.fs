open Suave                 // always open suave
open Suave.Http.Successful // for OK-result
open Suave.Web             // for config

web_server default_config (OK "Hello World from Suave!")