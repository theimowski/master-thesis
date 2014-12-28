open Suave                 // always open suave
open Suave.Http.Successful // for OK-result
open Suave.Web             // for config
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.RequestErrors
open Suave.Types

let contents = System.IO.File.ReadAllText("index.html")
let template = DotLiquid.Template.Parse(contents)

let HTML(container) = 
    fun (x : HttpContext) -> async {
        let dictionary = ["Container", container] |> dict
        return! OK (template.Render(DotLiquid.Hash.FromDictionary(dictionary))) x
    }

choose [
    GET >>= choose [
        url "/store" >>= (HTML "Hello from store")
        url "/store/browse" 
            >>= request(fun request -> cond (HttpRequest.query(request) ^^ "genre") 
                                            (fun genre -> HTML (sprintf "Genre: %s" genre)) 
                                            never)
        url_scan "/store/details/%d" (fun id -> HTML(sprintf "Details for id: %d" id))
    ]

    NOT_FOUND "404"
]
|> web_server default_config 