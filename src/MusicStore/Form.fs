module MusicStore.Form

open MusicStore.FormUtils

type Logon = {
    Username : string
    Password : string
}

let logon : Form<Logon> = Form ([],[])
let bindLogonForm2 = bindRequest logon

type Register = {
    Username : string
    Email : string
    Password : string
    ConfirmPassword : string
}

let passwordsMatch f = 
    f.Password = f.ConfirmPassword, "Passwords must match"
let register : Form<Register> = Form ([],[ passwordsMatch ])
let bindRegisterForm2 = bindRequest register

type Album = {
    ArtistId : int
    GenreId : int
    Title : string
    Price : decimal
    ArtUrl : string
}

let album : Form<Album> = 
    Form ([ StringProp ((fun f -> <@ f.Title @>), [ maxLength 100 ])
            StringProp ((fun f -> <@ f.ArtUrl @>), [ maxLength 100 ])
            DecimalProp ((fun f -> <@ f.Price @>), [ minimum 0.01M; maximum 100.0M; step 0.01M ])
            ],
          [])
let bindAlbumForm2 = bindRequest album

type Checkout = {
    FirstName : string
    LastName : string
    Address : string
    PromoCode : string option
}

let checkout : Form<Checkout> = Form ([], [])
let bindCheckoutForm2 = bindRequest checkout