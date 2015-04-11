module MusicStore.View

open System

open Suave.Html

open MusicStore.FormUtils
open MusicStore.FormHtml

let cssLink href = linkAttr [ "href", href; " rel", "stylesheet"; " type", "text/css" ]

let h1 xml = tag "h1" [] xml
let h2 s = tag "h2" [] (text s)
let h3 s = tag "h3" [] (text s)

let table x = tag "table" [] (flatten x)
let th x = tag "th" [] (flatten x)
let tr x = tag "tr" [] (flatten x)
let td x = tag "td" [] (flatten x)

let aHref href = tag "a" ["href", href]
let aHrefAttr href attr = tag "a" (("href", href) :: attr)
let imgSrc src = imgAttr [ "src", src ]
let divId id = divAttr ["id", id]
let divClass c = divAttr ["class", c]

let form x = tag "form" ["method", "POST"] (flatten x)
let fieldset x = tag "fieldset" [] (flatten x)
let legend txt = tag "legend" [] (text txt)
let submitInput value =
    inputAttr ["type", "submit"; "value", value]

let em s = tag "em" [] (text s)
let strong s = tag "strong" [] (text s)

let ulAnchors id links =
    tag "ul" ["id", id] (flatten [ for (href, xml) in links -> tag "li" [] (aHref href xml) ])

let truncate k (s : string) =
    if s.Length > k then
        s.Substring(0, k - 3) + "..."
    else s

let formatDec (d : Decimal) = d.ToString(Globalization.CultureInfo.InvariantCulture)

type Field = {
    Label : string
    Xml : Suave.Html.Xml
}

type Fieldset = {
    Legend : string
    Fields : Field list
}

type FormLayout<'a> = {
    Fieldsets : Fieldset list
    SubmitText : string
    Form : Form<'a>
}

let renderForm (layout : FormLayout<_>) =    
    
    form [
        for set in layout.Fieldsets -> 
            fieldset [
                yield legend set.Legend

                for field in set.Fields do
                    yield divClass "editor-label" [
                        text field.Label
                    ]
                    yield divClass "editor-field" [
                        field.Xml
                    ]
            ]

        yield submitInput layout.SubmitText
    ]


let viewAlbumDetails (album : Db.AlbumDetails) = [
    h2 album.Title
    p [ imgSrc album.AlbumArtUrl ]
    divId "album-details" [
        for (caption,t) in ["Genre:",album.Genre;"Artist:",album.Artist;"Price:",formatDec album.Price] ->
            p [
                em caption
                text t
            ]
        yield pAttr ["class", "button"] [
            aHref (sprintf Path.Cart.addAlbum album.AlbumId) (text "Add to cart")
        ]
    ]
]

let viewStore (genres : Db.Genre list) = [
    h3 "Browse Genres"
    p [ text "Select from genres:" ]
    ulAnchors "genre-list" [
        for g in genres -> Path.Store.browse |> Path.withParam (Path.Store.browseKey, g.Name), text g.Name
    ]
]

let viewHome (bestSellers : Db.BestSeller list) = [
    divId "promotion" [text " "]
    h3 "Fresh off the grill"
    ulAnchors "album-list" [
        for album in bestSellers -> 
            let href = sprintf Path.Store.details album.AlbumId
            let xml = flatten [ imgSrc "/placeholder.gif"; span (text album.Title)]
            href,xml
    ]
]

let viewAlbumsForGenre (genre : string) (albums : Db.Album list) = [ 
    divClass "genre" [ 
        h3 (genre + " Albums")
        ulAnchors "album-list" [
            for a in albums ->
            let href = sprintf Path.Store.details a.AlbumId
            let xml = flatten [ imgSrc "/placeholder.gif"; span (text a.Title) ]
            href,xml
        ]
    ]
]

let viewManageStore (albums : Db.AlbumDetails list) = [ 
    h2 "Index"
    p [aHref Path.Admin.createAlbum (text "Create New")]
    table [
        yield tr [
            for t in ["Artist";"Title";"Genre";"Price";""] -> th [ text t ]
        ]

        for album in albums -> 
        tr [
            for t in [ truncate 25 album.Artist; truncate 25 album.Title; album.Genre; formatDec album.Price ] ->
                td [ text t ]

            yield td [
                aHref (sprintf Path.Admin.editAlbum album.AlbumId) (text "Edit")
                text " | "
                aHref (sprintf Path.Admin.deleteAlbum album.AlbumId) (text "Delete")
            ]
        ]
    ]
]

let createEditAlbum (current : Db.Album option) header submit ((genres: Db.Genre list), (artists: Db.Artist list)) = 
    let artist, genre, title, price = 
        match current with
        | Some album -> Some album.ArtistId, Some album.GenreId, Some album.Title, Some (formatDec album.Price)
        | None -> None, None, None, None
    
    let optionalValue = Option.toList >> List.map (fun p -> "value", p)

    [ 
        h2 header
        
        renderForm
            { Fieldsets = 
                  [ { Legend = "Album"
                      Fields = 
                          [ { Label = "Genre"
                              Xml = selectInput Form.album (fun f -> <@ f.GenreId @>) (genres |> List.map (fun g -> g.GenreId, g.Name)) genre }
                            { Label = "Artist"
                              Xml = selectInput Form.album (fun f -> <@ f.ArtistId @>) (artists |> List.map (fun a -> a.ArtistId, a.Name)) artist }
                            { Label = "Title"
                              Xml = textInput Form.album (fun f -> <@ f.Title @>) (optionalValue title) }
                            { Label = "Price"
                              Xml = decimalInput Form.album (fun f -> <@ f.Price @>) (optionalValue price) }
                            { Label = "Album Art Url"
                              Xml = textInput Form.album (fun f -> <@ f.ArtUrl @>) (optionalValue (Some "placeholder.gif")) } ] } ]
              SubmitText = submit
              Form = Form.album }
    
        div [
            aHref Path.Admin.manage (text "Back to list")
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
        submitInput "Delete"
    ]

    div [
        aHref Path.Admin.manage (text "Back to list")
    ]
]

let viewLogon msg = [
    h2 "Log On"
    p [
        text "Please enter your user name and password."
        aHref Path.Account.register (text "Register")
        text " if you don't have an account yet."
    ]

    divId "logon-message" [
        text (defaultArg msg " ")
    ]

    renderForm
        { Fieldsets = 
              [ { Legend = "Account Information"
                  Fields = 
                      [ { Label = "User Name"
                          Xml = textInput Form.logon (fun f -> <@ f.Username @>) [] }
                        { Label = "Password"
                          Xml = passwordInput Form.logon (fun f -> <@ f.Password @>) [] } ] } ]
          SubmitText = "Log On"
          Form = Form.logon }
]

let viewRegister = [
    h2 "Create a New Account"
    p [
        text "Use the form below to create a new account."
    ]

    renderForm
        { Fieldsets = 
              [ { Legend = "Create a New Account"
                  Fields = 
                      [ { Label = "User name"
                          Xml = textInput Form.register (fun f -> <@ f.Username @>) [] }
                        { Label = "Email address (optional)"
                          Xml = optionalEmailInput Form.register (fun f -> <@ f.Email @>) [] }
                        { Label = "Password (between 6 and 20 characters)"
                          Xml = passwordInput Form.register (fun f -> <@ f.Password @>) [] }
                        { Label = "Confirm password"
                          Xml = passwordInput Form.register (fun f -> <@ f.ConfirmPassword @>) [] } ] } ]
          SubmitText = "Register"
          Form = Form.register }

]

let viewEmptyCart = [
    h3 "Your cart is empty"
    text "Find some great music in our "
    aHref Path.home (text "store")
    text "!"
]

let viewNonEmptyCart (carts : Db.CartDetails list) = [
    h3 "Review your cart:"
    pAttr ["class", "button"] [
            aHref Path.Cart.checkout (text "Checkout >>")
    ]
    divId "update-message" [text " "]
    table [
        yield tr [
            for h in ["Album Name"; "Price (each)"; "Quantity"; ""] ->
            th [text h]
        ]
        for cart in carts ->
            tr [
                td [
                    aHref (sprintf Path.Store.details cart.AlbumId) (text cart.AlbumTitle)
                ]
                td [
                    text (formatDec cart.Price)
                ]
                td [
                    text (cart.Count.ToString())
                ]
                td [
                    aHrefAttr "#" ["class", "removeFromCart"; "data-id", cart.AlbumId.ToString()] (text "Remove from cart") 
                ]
            ]
        yield tr [
            for d in ["Total"; ""; ""; carts |> List.sumBy (fun c -> c.Price * (decimal c.Count)) |> formatDec] ->
            td [text d]
        ]
    ]
    scriptAttr [ "type", "text/javascript"; " src", "/jquery-1.11.2.js" ] [ text ";" ]
    scriptAttr [ "type", "text/javascript"; " src", "/script.js" ] [ text ";" ]
]

let viewCart = function
    | [] -> viewEmptyCart
    | list -> viewNonEmptyCart list

let viewCheckout = [
    h2 "Address And Payment"
    renderForm
            { Fieldsets = 
                  [ { Legend = "Shipping Information"
                      Fields = 
                          [ { Label = "First Name"
                              Xml = textInput Form.checkout (fun f -> <@ f.FirstName @>) [] }
                            { Label = "Last Name"
                              Xml = textInput Form.checkout (fun f -> <@ f.LastName @>) [] }
                            { Label = "Address"
                              Xml = textInput Form.checkout (fun f -> <@ f.Address @>) [] } ] }
                    
                    { Legend = "Payment"
                      Fields = 
                          [ { Label = "Promo Code"
                              Xml = optionalTextInput Form.checkout (fun f -> <@ f.PromoCode @>) [] } ] } ]
              SubmitText = "Submit Order"
              Form = Form.checkout }
]

let viewCheckoutComplete orderId = [
    h2 "Checkout Complete"
    p [
        text (sprintf "Thanks for your order! Your order number is: %d" orderId)
    ]
    p [
        text "How about shopping for some more music in our "
        aHref Path.home (text "store")
        text "?"
    ]
]

let partCategories (genres : Db.Genre list) =
    ulAnchors "categories" [
                for genre in genres -> 
                    Path.Store.browse |> Path.withParam (Path.Store.browseKey, genre.Name), text genre.Name
            ]

let partUser (user : string option) = 
    divId "part-user" [
        match user with
        | Some user -> 
            yield text (sprintf "Logged on as %s, " user)
            yield aHref Path.Account.logoff (text "Log off")
        | None ->
            yield aHref Path.Account.logon (text "Log on")
    ] 

let partNav userRole itemsNumber = 
    ulAnchors "navlist" [ 
                    yield Path.home, text "Home"
                    yield Path.Store.overview, text "Store"
                    yield Path.Cart.overview, text (sprintf "Cart (%d)" itemsNumber)
                    if userRole = Some "admin" then
                        yield Path.Admin.manage, text "Admin"
                ]

let viewIndex partUser partCategories partNav container = 
    html [ 
        head [
            title "F# Suave Music Store"
            cssLink "/Site.css"
        ] 
       
        body [
            divId "header" [
                h1 (aHref Path.home (text "F# Suave Music Store"))
                partNav
                partUser
            ]

            partCategories

            divId "container" container
            
            divId "footer" [
                text "built with "
                aHref "http://fsharp.org" (text "F#")
                text " and "
                aHref "http://suave.io" (text "Suave.IO")
            ]
        ]
    ]