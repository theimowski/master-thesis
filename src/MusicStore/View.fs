module MusicStore.View

open System

open Suave.Html

let cssLink href = 
    linkAttr [ "href", href; "rel", "stylesheet"; "type", "text/css" ]

let h1 xml = tag "h1" [] xml
let h2 s = tag "h2" [] (text s)
let h3 s = tag "h3" [] (text s)

let table x = tag "table" [] (flatten x)
let th x = tag "th" [] (flatten x)
let tr x = tag "tr" [] (flatten x)
let td x = tag "td" [] (flatten x)

let aHref href = tag "a" ["href", href]
let imgSrc src = imgAttr [ "src", src ]
let divId id = divAttr ["id", id]
let divClass c = divAttr ["class", c]

let form x = tag "form" ["method", "POST"] (flatten x)
let fieldset x = tag "fieldset" [] (flatten x)
let legend txt = tag "legend" [] (text txt)
let select name x = tag "select" ["name", name] (flatten x)
let option value txt selected =
    if selected then
        tag "option" ["value", value; "selected", "selected"] (text txt)
    else
        tag "option" ["value", value] (text txt)
let textInput name value attrs = 
    let v = match value with Some v -> v | _ -> ""
    inputAttr (["name", name; "type", "text"; "value", v; "required", ""] @ attrs)
let numberInput name value attrs = 
    let v = match value with Some v -> v | _ -> ""
    inputAttr (["name", name; "type", "number"; "value", v; "required", ""] @ attrs)
let passwordInput name =
    inputAttr ["name", name; "type", "password"; "required", ""]

let em s = tag "em" [] (text s)
let strong s = tag "strong" [] (text s)

let ulAnchors id links =
    tag "ul" ["id", id] (flatten [ for (href, xml) in links -> tag "li" [] (aHref href xml) ])

let truncate k (s : string) =
    if s.Length > k then
        s.Substring(0, k - 3) + "..."
    else s

let formatDec (d : Decimal) = d.ToString(Globalization.CultureInfo.InvariantCulture)

let viewAlbumDetails (album : Db.AlbumDetails) = [
    h2 album.Title
    p [ imgSrc "/placeholder.gif" ]
    divId "album-details" [
        for (caption,t) in ["Genre:",album.Genre;"Artist:",album.Artist;"Price:",formatDec album.Price] ->
        p [
            em caption
            text t
        ]
    ]
]

let viewStore (genres : Db.Genre list) = [
    h3 "Browse Genres"
    p [ text "Select from genres:" ]
    ulAnchors "genre-list" [
        for g in genres -> sprintf "/store/browse?genre=%s" g.Name, text g.Name
    ]
]

let viewHome () = [
    divId "promotion" []
]

let viewAlbumsForGenre (genre : Db.Genre, albums : Db.Album list) = [ 
    divClass "genre" [ 
        h3 (genre.Name + " Albums")
        ulAnchors "album-list" [
            for a in albums ->
            let href = sprintf "/store/details/%d" a.AlbumId
            let xml = flatten [ imgSrc "/placeholder.gif"; span (text a.Title) ]
            href,xml
        ]
    ]
]

let viewManageStore (albums : Db.AlbumDetails list) = [ 
    h2 "Index"
    p [aHref "/store/manage/create" (text "Create New")]
    table [
        yield tr [
            for t in ["Artist";"Title";"Genre";"Price";""] -> th [ text t ]
        ]

        for album in albums -> 
        tr [
            for t in [ truncate 25 album.Artist; truncate 25 album.Title; album.Genre; formatDec album.Price ] ->
                td [ text t ]

            yield td [
                aHref (sprintf "/store/manage/edit/%d" album.AlbumId) (text "Edit")
                text " | "
                aHref (sprintf "/store/manage/delete/%d" album.AlbumId) (text "Delete")
            ]
        ]
    ]
]

let createEditAlbum (current : Db.AlbumDetails option) header submit ((genres: Db.Genre list), (artists: Db.Artist list)) = 
    let artist, genre, title, price = 
        match current with
        | Some album -> Some album.Artist, Some album.Genre, Some album.Title, Some (formatDec album.Price)
        | None -> None, None, None, None
    
    [ 
        h2 header
    
        form [
            fieldset [
                legend "Album"
                
                div [ text "Genre" ]
                div [ select "genre" [
                        for g in genres -> option (string g.GenreId) g.Name (Some g.Name = genre) ] ]
                div [ text "Artist" ]
                div [ select "artist" [
                        for a in artists -> option (string a.ArtistId) a.Name (Some a.Name = artist) ] ]
                div [ text "Title" ]
                div [ textInput "title" title ["maxlength", "100"] ]
                div [ text "Price" ]
                div [ numberInput "price" price ["step", "0.01";  "min", "0.01"; "max", "100.00"] ]
                div [ text "Album Art Url" ]
                div [ textInput "artUrl" (Some "placeholder.gif") ["maxlength", "100"] ]

                p [ inputAttr ["type", "submit"; "value", submit] ]  
            ]
        ]
    
        div [
            aHref "/store/manage" (text "Back to list")
        ]
    ]

let viewCreateAlbum = createEditAlbum None "Create" "Create" 
let viewEditAlbum (album,genres,artists) = createEditAlbum (Some album) "Edit" "Save" (genres,artists)

let viewDeleteAlbum (album : Db.AlbumDetails) = [
    h2 "Delete Confirmation"
    p [ 
        text "Are you sure you want to delete the album titled"
        br
        strong album.Title
        text "?"
    ]
    
    form [
        inputAttr ["type", "submit"; "value", "Delete"]
    ]

    div [
        aHref "/store/manage" (text "Back to list")
    ]
]

let viewLogon () = [
    h2 "Log On"
    p [
        text "Please enter your user name and password."
    ]

    form [
        fieldset [
            legend "Account Information"
            
            div [ 
                text "User name" 
            ]
            divClass "editor-field" [ 
                textInput "username" None []
            ]

            div [ 
                text "Password" 
            ]
            divClass "editor-field" [ 
                passwordInput "password"
            ]

            p [
               inputAttr ["type", "submit"; "value", "Log On"]
            ]
        ]
    ]
]

let viewIndex (genres : Db.Genre list) xml = 
    html [ 
        head [
            title "F# Suave Music Store"
            cssLink "/Site.css"
        ] 
       
        body [
            divId "header" [
                h1 (aHref "/" (text "F# Suave Music Store"))
                ulAnchors "navlist" [ 
                    "/", text "Home"
                    "/store", text "Store"
                    "/store/manage", text "Admin"
                ]
            ]

            ulAnchors "categories" [
                for genre in genres -> sprintf "/store/browse?genre=%s" genre.Name, text genre.Name
            ]

            divId "container" xml
            
            divId "footer" [
                text "built with "
                aHref "http://fsharp.org" (text "F#")
                text " and "
                aHref "http://suave.io" (text "Suave.IO")
            ]
        ]
    ]