module MusicStore.Form

open MusicStore.FormUtils

type X = string * FormField

type XS = X list

type FormLayout = {
    Fieldsets : (string * XS) list
    SubmitText : string
}

module Logon = 
    let Username = TextField("username", [ MaxLength 20 ])
    let Password = PasswordField("password", [])
    
    let form = {
        Fields = [TextFormField Username; PasswordFormField Password]
        ServerSideValidations = []
    }
        
let logonForm req = bindingForm Logon.form req

module Register =
    let Username = TextField("username", [])
    let Email = TextField("email", [])
    let Password = PasswordField("password", [])
    let ConfirmPassword = PasswordField("confirmpassword", [])

    let passwordsMatch result =
        result.GetPassword Password = result.GetPassword ConfirmPassword, "Passwords must match"
    
    let form = 
        { Fields = 
              [ TextFormField Username
                TextFormField Email
                PasswordFormField Password
                PasswordFormField ConfirmPassword ]
          ServerSideValidations = [ passwordsMatch ] }

let registerForm req = bindingForm Register.form req

module Album =
    let ArtistId = IntegerField("artist", [])
    let GenreId = IntegerField("genre", [])
    let Title = TextField("title", [ MaxLength 100 ])
    let Price = DecimalField("price", [ Minimum 0.01M; Maximum 100.0M; Step 0.01M ])
    let ArtUrl = TextField("artUrl", [ MaxLength 100 ])

    let form = 
        { Fields = 
              [ IntegerFormField ArtistId
                IntegerFormField GenreId
                TextFormField Title
                DecimalFormField Price
                TextFormField ArtUrl ]
          ServerSideValidations = [] }

let albumForm req = bindingForm Album.form req

module Checkout =
    let FirstName = TextField("firstname", [])
    let LastName = TextField("lastname", [])
    let Address = TextField("address", [])
    let PromoCode = TextField("promocode", [])

    let form = 
        { Fields =
            [ TextFormField FirstName
              TextFormField LastName
              TextFormField Address
              TextFormField PromoCode ]
          ServerSideValidations = [] }

let checkoutForm req = bindingForm Checkout.form req