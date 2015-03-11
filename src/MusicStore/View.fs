module MusicStore.View

open System

open Suave.Html

let h1 xml = tag "h1" [] xml
let h2 s = tag "h2" [] (Xml([Text s,Xml []]))
let h3 s = tag "h3" [] (Xml([Text s,Xml []]))

let table = tag "table" []
let th = tag "th" []
let td = tag "td" []
let tr = tag "tr" []

let anchor href = tag "a" ["href", href]

let imgSrc src = imgAttr [ "src", src ]

let divId id = divAttr ["id", id]

let form xml = tag "form" ["method", "POST"] xml

let em s = tag "em" [] (Xml([Text s, Xml []]))
let strong s = tag "strong" [] (text s)

let liAnchor (href, xml) =
    tag "li" [] (anchor href xml)

let ulAnchors id links =
    tag "ul" ["id", id] (links |> List.map liAnchor |> flatten)

let cssLink href = 
    linkAttr [ "href", href; "rel", "stylesheet"; "type", "text/css" ]

let truncate k (s : string) =
    if s.Length > k then
        s.Substring(0, k - 3) + "..."
    else s

let formatDec (d : Decimal) = d.ToString(Globalization.CultureInfo.InvariantCulture)

let viewAlbumDetails ((album, artist, genre) : Db.AlbumDetails) = [
    h2 album.Title
    p [ imgSrc "/placeholder.gif" ]
    divId "album-details" [
        p [
            em "Genre:"
            text genre.Name
        ]
        p [
            em "Artist:"
            text artist.Name
        ]
        p [
            em "Price:"
            text (formatDec album.Price)
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

let viewAlbumsForGenre (genre : Db.Genre, albums : Db.Album list) = 
    let item (a : Db.Album) = 
        let href = "/store/details/" + a.AlbumId.ToString()
        let xml = [ imgSrc "/placeholder.gif"
                    span (text a.Title) ] |> flatten
        href,xml

    [ divAttr ["class", "genre"] 
        [ h3 (genre.Name + " Albums")
          ulAnchors "album-list" (List.map item albums)
        ]
    ]

let viewManageStore (albums : Db.AlbumDetails list) = 
    let headers = 
        ["Genre";"Artist";"Title";"Price";""]
        |> List.map (text >> th)
        |> flatten
        |> tr

    let actions (Db.Album a) =
        [ anchor (sprintf "/store/manage/edit/%d" a.AlbumId) (text "Edit")
          text " | "
          anchor (sprintf "/store/manage/delete/%d" a.AlbumId) (text "Delete")
        ] 
        |> flatten
        |> td

    let details ((album, artist, genre) : Db.AlbumDetails) =
        [ genre.Name
          artist.Name |> truncate 25
          album.Title |> truncate 25
          formatDec album.Price 
        ] |> List.map (text >> td)

    let row (albumDetails : Db.AlbumDetails) =
        List.append (details albumDetails) [actions albumDetails]
        |> flatten
        |> tr

    [ h2 "Index"
      p [anchor "/store/manage/create" (text "Create New")]
      table (headers :: (List.map row albums) |> flatten)
    ]


let createEditAlbum (current : Db.AlbumDetails option) caption submit ((g: Db.Genre list), (a: Db.Artist list)) = 

    let artist = current |> Option.map (fun (Db.Artist a) -> a.Name)
    let genre = current |> Option.map (fun (Db.Genre g) -> g.Name)
    let title, price = 
        match current with
        | Some (Db.Album a) -> a.Title, formatDec a.Price
        | None -> "", ""

    let opt (id : int,name,current) =
        let attrs =
            match current with
            | Some x when x = name ->
                ["value", id.ToString(); "selected", "selected"]
            | _ -> 
                ["value", id.ToString()]
        tag "option" attrs (text name)

    let fields = 
        [
        text "Genre"
        tag "select" ["name", "genre"] (g |> List.map (fun g -> g.GenreId,g.Name,genre) |> List.map opt |> flatten)
        text "Artist"
        tag "select" ["name", "artist"] (a |> List.map (fun a -> a.ArtistId,a.Name,artist) |> List.map opt |> flatten)
        text "Title"
        inputAttr ["name", "title"; "type", "text"; "required", ""; "value", title; "maxlength", "100"]
        text "Price"
        inputAttr ["name", "price"; "type", "number"; "required", ""; "value", price; "step", "0.01"]
        text "Album Art Url"
        inputAttr ["name", "artUrl"; "type", "text"; "required", ""; "value", "placeholder.gif"; "maxlength", "100"; "min", "0.01"; "max", "100.00"]
        ] 
        |> List.map (Seq.singleton >> List.ofSeq >> div)
        |> flatten
         

    let fieldset = 
        [ tag "legend" [] (text "Album")
          fields
          p [inputAttr ["type", "submit"; "value", submit]]
        ] |> flatten
    
    [
    h2 caption
    
    tag "form" ["method","POST"] 
        (tag "fieldset" [] fieldset)
    
    div [
        anchor "/store/manage" (text "Back to list")
    ]
]

let viewCreateAlbum = createEditAlbum None "Create" "Create" 
let viewEditAlbum (album,g,a) = createEditAlbum (Some album) "Edit" "Save" (g,a)

let viewDeleteAlbum (Db.Album a) = [
    h2 "Delete Confirmation"
    p [ 
        text "Are you sure you want to delete the album titled"
        br
        strong a.Title
        text "?"
    ]
    form (inputAttr ["type", "submit"; "value", "Delete"])

    div [
        anchor "/store/manage" (text "Back to list")
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
                h1 (anchor "/" (text "F# Suave Music Store"))
                ulAnchors "navlist" [ 
                    "/", text "Home"
                    "/store", text "Store"
                    "/store/manage", text "Admin"
                ]  
            ]

            ulAnchors "categories" [
                for g in genres -> sprintf "/store/browse?genre=%s" g.Name, text g.Name
            ]

            divId "container" xml

            divId "footer" [
                text "built with "
                anchor "http://fsharp.org" (text "F#")
                text " and "
                anchor "http://suave.io" (text "Suave.IO")
            ]
        ]
    ]