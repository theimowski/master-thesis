module MusicStore.View

open System

open MusicStore.Domain

open DotLiquid
open Suave.Html

type Index = {
    Container : string
    Genres : Genre []
}

type Home = { 
    Placeholder : unit
}

type Store = {
    Genres : Genre []
}

type AlbumDetails = {
    Album : Album
}

type AlbumsForGenre = {
    Genre : Genre
    Albums : IdAndName [] 
}

type ManageStore = {
    Albums : Album []
}

type CreateAlbum = {
    Artists : Artist []
    Genres : Genre []
}

type EditAlbum = {
    Artists : Artist []
    Genres : Genre []
    Album : Album
}

type DeleteAlbum = {
    Album : Album
}

let registerType (modelType : Type) = 
    let fields = Reflection.FSharpType.GetRecordFields(modelType) |> Array.map (fun f -> f.Name)
    Template.RegisterSafeType(modelType, fields)

let templates =
    registerType typeof<IdAndName>
    registerType typeof<Album>
    let views = typeof<Index>.DeclaringType.GetNestedTypes()
    views |> Array.iter registerType
    views 
    |> Array.map (fun view -> view.Name, Template.Parse(IO.File.ReadAllText(view.Name + ".html")))
    |> dict

let render model =  
    let template = templates.[model.GetType().Name]
    template.Render(DotLiquid.Hash.FromAnonymousObject(model))


let truncate k (s : string) =
    if s.Length > k then
        s.Substring(0, k - 3) + "..."
    else s

let vAD (ad : AlbumDetails) = [
    tag "h2" [] (text ad.Album.Title)
    p [ imgAttr [ "src", "/placeholder.gif"] ]
    divAttr ["id", "album-details"] [
        p [
            tag "em" [] (text "Genre:")
            text ad.Album.Genre
        ]
        p [
            tag "em" [] (text "Artist:")
            text ad.Album.Artist
        ]
        p [
            tag "em" [] (text "Price:")
            text ad.Album.Price
        ]
    ]
]

let vS (s : Store) = [
    tag "h3" [] (text "Browse Genres")
    p [ text "Select from genres:" ]
    tag "ul" [] (s.Genres |> Array.map (fun g -> tag "li" [] (tag "a" ["href", "/store/browse?genre=" + g.Name] (text g.Name) ) ) |> Array.toList |> flatten)
]

let vHome () = [
    divAttr ["id", "promotion"] []
]

let vAlbumsForGenre (a : AlbumsForGenre) = 
    let item (a : IdAndName) = 
        tag "li" [] (tag "a" ["href", "/store/details/" + a.Id.ToString()]  
                        ([
                            imgAttr ["src", "/placeholder.gif"]
                            span (text a.Name)
                         ] |> flatten))

    [divAttr ["class", "genre"] [
        tag "h3" [] (flatten [tag "em" [] (text a.Genre.Name); text " Albums"])
        tag "ul" ["id", "album-list"] (a.Albums |> Array.toList |> List.map item |> flatten)
    ]
]

let vManageStore (m : ManageStore) = 
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
     tag "table" [] (List.append [headers] (List.map row (m.Albums |> Array.toList)) |> flatten)
    ]

let vDeleteAlbum (d : DeleteAlbum) = [
    tag "h2" [] (text "Delete Confirmation")
    p [ text "Are you sure you want to delete the album titled"
        br
        tag "strong" [] (text d.Album.Title)
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

            tag "ul" ["id", "categories"] (genres |> Array.map (fun g -> tag "li" [] (tag "a" ["href", "/store/browse?genre=" + g.Name] (text g.Name) ) ) |> Array.toList |> flatten)

            divAttr ["id", "container"] xml

            divAttr ["id", "footer"] [
                text "built with "
                tag "a" ["href", "http://fsharp.org"] (text "F#")
                text " and "
                tag "a" ["href", "http://suave.io"] (text "Suave.IO")
            ]
        ]
    ]