module MusicStore.View

open System

open MusicStore.Domain

open Suave.Html

let truncate k (s : string) =
    if s.Length > k then
        s.Substring(0, k - 3) + "..."
    else s

let viewAlbumDetails (album : Album) = [
    tag "h2" [] (text album.Title)
    p [ imgAttr [ "src", "/placeholder.gif"] ]
    divAttr ["id", "album-details"] [
        p [
            tag "em" [] (text "Genre:")
            text album.Genre
        ]
        p [
            tag "em" [] (text "Artist:")
            text album.Artist
        ]
        p [
            tag "em" [] (text "Price:")
            text album.Price
        ]
    ]
]

let viewStore (genres : IdAndName list) = [
    tag "h3" [] (text "Browse Genres")
    p [ text "Select from genres:" ]
    tag "ul" [] (genres |> List.map (fun g -> tag "li" [] (tag "a" ["href", "/store/browse?genre=" + g.Name] (text g.Name) ) ) |> flatten)
]

let vHome () = [
    divAttr ["id", "promotion"] []
]

let viewAlbumsForGenre (genre, albums : IdAndName list) = 
    let item (a : IdAndName) = 
        tag "li" [] (tag "a" ["href", "/store/details/" + a.Id.ToString()]  
                        ([
                            imgAttr ["src", "/placeholder.gif"]
                            span (text a.Name)
                         ] |> flatten))

    [divAttr ["class", "genre"] [
        tag "h3" [] (flatten [tag "em" [] (text genre); text " Albums"])
        tag "ul" ["id", "album-list"] (albums |> List.map item |> flatten)
    ]
]

let vManageStore (albums : Album list) = 
    let headers = 
        ["Genre";"Artist";"Title";"Price";""]
        |> List.map (text >> tag "th" [])
        |> flatten
        |> tag "tr" []

    let actions (a: Album) =
        [ tag "a" ["href", "/store/manage/edit/" + a.Id.ToString()] (text "Edit")
          text " | "
          tag "a" ["href", "/store/manage/delete/" + a.Id.ToString()] (text "Delete")
        ] 
        |> flatten
        |> tag "td" []

    let details (a : Album) =
        [a.Genre; a.Artist |> truncate 25; a.Title |> truncate 25; a.Price]
        |> List.map (text >> tag "td" [])

    let row (a: Album) =
        List.append (details a) [actions a]
        |> flatten
        |> tag "tr" []

    [tag "h2" [] (text "Index")
     p [tag "a" ["href", "/store/manage/create"] (text "Create New")]
     tag "table" [] (List.append [headers] (List.map row albums) |> flatten)
    ]


let createEditAlbum (current : Album option) caption submit ((g: IdAndName list), (a: IdAndName list)) = 

    let artist = current |> Option.map (fun a -> a.Artist)
    let genre = current |> Option.map (fun a -> a.Genre)
    let title = 
        match current with
        | Some a -> a.Title
        | None -> ""
    let price = 
        match current with 
        | Some a -> a.Price
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
        tag "select" ["name", "genre"] (g |> List.map (fun g -> g.Id,g.Name,genre) |> List.map opt |> flatten)
        text "Artist"
        tag "select" ["name", "artist"] (a |> List.map (fun g -> g.Id,g.Name,artist) |> List.map opt |> flatten)
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
    tag "h2" [] (text caption)
    
    tag "form" ["method","POST"] 
        (tag "fieldset" [] fieldset)
    
    div [
        tag "a" ["href","/store/manage"] (text "Back to list")
    ]
]

let vCreateAlbum = createEditAlbum None "Create" "Create" 
let vEditAlbum (album,g,a) = createEditAlbum (Some album) "Edit" "Save" (g,a)

let vDeleteAlbum (a : Album) = [
    tag "h2" [] (text "Delete Confirmation")
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

let index genres xml = 
    html [ 
        head [
            title "F# Suave Music Store"
            linkAttr [ "href", "/Site.css"; "rel", "stylesheet"; "type", "text/css" ]
        ] 
        body [
            divAttr ["id", "header"] [
                tag "h1" [] (tag "a" ["href", "/"] (text "F# Suave Music Store"))
                tag "ul" ["id", "navlist"] 
                    (flatten [
                        tag "li" ["class", "first"] (tag "a" ["id", "current"; "href", "/"] (text "Home"))
                        tag "li" [] (tag "a" ["href", "/store"] (text "Store"))
                        tag "li" [] (tag "a" ["href", "/store/manage"] (text "Admin"))
                    ])
                ]

            tag "ul" ["id", "categories"] (genres |> List.map (fun g -> tag "li" [] (tag "a" ["href", "/store/browse?genre=" + g.Name] (text g.Name) ) ) |> flatten)

            divAttr ["id", "container"] xml

            divAttr ["id", "footer"] [
                text "built with "
                tag "a" ["href", "http://fsharp.org"] (text "F#")
                text " and "
                tag "a" ["href", "http://suave.io"] (text "Suave.IO")
            ]
        ]
    ]