module MusicStore.View

open System

open Suave.Html

let h2Attr attr s = tag "h2" attr (Xml([Text s,Xml []]))
let h2  = h2Attr [ ]

let h3Attr attr s = tag "h3" attr (Xml([Text s,Xml []]))
let h3  = h3Attr [ ]

let imgSrc src = imgAttr [ "src", src ]

let divId id = divAttr ["id", id]

let formatDec (d : Decimal) = d.ToString(Globalization.CultureInfo.InvariantCulture)

let em s = tag "em" [] (Xml([Text s, Xml []]))

let truncate k (s : string) =
    if s.Length > k then
        s.Substring(0, k - 3) + "..."
    else s

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
    tag "ul" [] (genres |> List.map (fun g -> tag "li" [] (tag "a" ["href", "/store/browse?genre=" + g.Name] (text g.Name) ) ) |> flatten)
]

let viewHome () = [
    divId "promotion" []
]

let viewAlbumsForGenre (genre : Db.Genre, albums : Db.Album list) = 
    let item (a : Db.Album) = 
        tag "li" [] (tag "a" ["href", "/store/details/" + a.AlbumId.ToString()]  
                        ([
                            imgSrc "/placeholder.gif"
                            span (text a.Title)
                         ] |> flatten))

    [divAttr ["class", "genre"] [
        h3 (genre.Name + " Albums")
        tag "ul" ["id", "album-list"] (albums |> List.map item |> flatten)
    ]
]

let viewManageStore (albums : Db.AlbumDetails list) = 
    let headers = 
        ["Genre";"Artist";"Title";"Price";""]
        |> List.map (text >> tag "th" [])
        |> flatten
        |> tag "tr" []

    let actions (a: Db.Album) =
        [ tag "a" ["href", "/store/manage/edit/" + a.AlbumId.ToString()] (text "Edit")
          text " | "
          tag "a" ["href", "/store/manage/delete/" + a.AlbumId.ToString()] (text "Delete")
        ] 
        |> flatten
        |> tag "td" []

    let details ((album, artist, genre) : Db.AlbumDetails) =
        [genre.Name; artist.Name |> truncate 25; album.Title |> truncate 25; formatDec album.Price]
        |> List.map (text >> tag "td" [])

    let row ((album, _, _) as albumDetails : Db.AlbumDetails) =
        List.append (details albumDetails) [actions album]
        |> flatten
        |> tag "tr" []

    [h2 "Index"
     p [tag "a" ["href", "/store/manage/create"] (text "Create New")]
     tag "table" [] (List.append [headers] (List.map row albums) |> flatten)
    ]


let createEditAlbum (current : Db.AlbumDetails option) caption submit ((g: Db.Genre list), (a: Db.Artist list)) = 

    let artist = current |> Option.map (fun (_,a,_) -> a.Name)
    let genre = current |> Option.map (fun (_,_,g) -> g.Name)
    let title = 
        match current with
        | Some (a,_,_) -> a.Title
        | None -> ""
    let price = 
        match current with 
        | Some (a,_,_) -> formatDec a.Price
        | None -> ""

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
        tag "a" ["href","/store/manage"] (text "Back to list")
    ]
]

let viewCreateAlbum = createEditAlbum None "Create" "Create" 
let viewEditAlbum (album,g,a) = createEditAlbum (Some album) "Edit" "Save" (g,a)

let viewDeleteAlbum ((a, _, _) as albumDetails : Db.AlbumDetails) = [
    h2 "Delete Confirmation"
    p [ text "Are you sure you want to delete the album titled"
        br
        tag "strong" [] (text a.Title)
        text "?"
    ]
    tag "form" ["method","POST"] 
        (p [inputAttr ["type", "submit"; "value", "Delete"]])

    div [
        tag "a" ["href","/store/manage"] (text "Back to list")
    ]
]

let viewIndex (genres : Db.Genre list) xml = 
    html [ 
        head [
            title "F# Suave Music Store"
            linkAttr [ "href", "/Site.css"; "rel", "stylesheet"; "type", "text/css" ]
        ] 
        body [
            divId "header" [
                tag "h1" [] (tag "a" ["href", "/"] (text "F# Suave Music Store"))
                tag "ul" ["id", "navlist"] 
                    (flatten [
                        tag "li" ["class", "first"] (tag "a" ["id", "current"; "href", "/"] (text "Home"))
                        tag "li" [] (tag "a" ["href", "/store"] (text "Store"))
                        tag "li" [] (tag "a" ["href", "/store/manage"] (text "Admin"))
                    ])
                ]

            tag "ul" ["id", "categories"] (genres |> List.map (fun g -> tag "li" [] (tag "a" ["href", "/store/browse?genre=" + g.Name] (text g.Name) ) ) |> flatten)

            divId "container" xml

            divId "footer" [
                text "built with "
                tag "a" ["href", "http://fsharp.org"] (text "F#")
                text " and "
                tag "a" ["href", "http://suave.io"] (text "Suave.IO")
            ]
        ]
    ]