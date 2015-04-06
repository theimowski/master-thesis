module MusicStore.Form

open MusicStore.FormUtils

type Logon = {
    Username : string
    Password : string
}

let logon : Form<Logon> = Form ([],[])

type Register = {
    Username : string
    Email : Email option
    Password : string
    ConfirmPassword : string
}

let pattern = @"(\w){6,20}"

let passwordsMatch f = 
    f.Password = f.ConfirmPassword, "Passwords must match"

let register : Form<Register> = 
    Form ([ StringProp ((fun f -> <@ f.Username @>), [ maxLength 30 ] )
            StringProp ((fun f -> <@ f.Password @>), [ matches pattern ] )
            StringProp ((fun f -> <@ f.ConfirmPassword @>), [ matches pattern ] )
            ],[ passwordsMatch ])

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

type Checkout = {
    FirstName : string
    LastName : string
    Address : string
    PromoCode : string option
}

let checkout : Form<Checkout> = Form ([], [])