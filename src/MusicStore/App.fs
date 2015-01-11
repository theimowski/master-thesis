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

open MusicStore.Domain
open MusicStore.Db
open MusicStore.View

let private parse_using<'a> (f:string -> bool * 'a) s =
    match f s with
    | true, i -> Choice1Of2 i
    | false, _ -> Choice2Of2 (sprintf "Cound not parse '%s' to %s" s typeof<'a>.Name)

module Parse = 
    let decimal = 
        parse_using 
            (fun s -> 
            Decimal.TryParse
                (s, Globalization.NumberStyles.AllowDecimalPoint, 
                 Globalization.CultureInfo.InvariantCulture))

let bindForm key = Binding.form key Choice1Of2

let albumForm req = binding {
        let! artistId = req |> bindForm "artist" >>. Parse.int32
        let! genreId = req |> bindForm "genre" >>. Parse.int32
        let! title = req |> bindForm "title"
        let! price = req |> bindForm "price" >>. Parse.decimal
        let! artUrl = req |> bindForm "artUrl"

        return artistId, genreId, title, price, artUrl
    }

let get getF (x: HttpContext) = async {
        let ctx = sql.GetDataContext()
        let genres = (getStore ctx).Genres
        let model = getF ctx
        
        let container = View.render(model)
        let index = {Index.Container = container; Genres = genres}
        return! (OK (View.render index) >>= Writers.set_mime_type "text/html; charset=utf-8") x
    }

let post postF (x: HttpContext) = async {
        let ctx = sql.GetDataContext()
        postF ctx
        return! Redirection.redirect "/store/manage" x   
    }

choose [
    GET >>= choose [
        url "/" >>= get getHome
        url "/store" >>= get getStore
        url "/store/browse" 
            >>= Binding.bind_req 
                    (Binding.query "genre" Choice1Of2) 
                    (getGenre >> get) 
                    BAD_REQUEST
        url_scan "/store/details/%d" (getAlbum >> get)

        url "/store/manage" >>= get getManageStore
        url "/store/manage/create" >>= get getCreateAlbum
        url_scan "/store/manage/edit/%d" (getEditAlbum >> get)
        url_scan "/store/manage/delete/%d" (getDeleteAlbum >> get)

        url_regex "(.*?)\.(?!js$|css$|png$|gif$).*" >>= RequestErrors.FORBIDDEN "Access denied."
        Files.browse'
    ]

    POST >>= ParsingAndControl.parse_post_data >>= choose [
        url "/store/manage/create" 
            >>= (Binding.bind_req 
                    (albumForm >> Choice.map CreateAlbumCommand.create) 
                    (createAlbum >> post) 
                    BAD_REQUEST)
        url_scan "/store/manage/edit/%d" 
            (fun id -> 
                Binding.bind_req 
                    (albumForm >> Choice.map (UpdateAlbumCommand.create id)) 
                    (updateAlbum >> post) 
                    BAD_REQUEST)
        url_scan "/store/manage/delete/%d" (deleteAlbum >> post)
    ]

    NOT_FOUND "404"
]
|> web_server default_config 