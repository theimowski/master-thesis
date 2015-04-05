module MusicStore.Form

open MusicStore.FormUtils
open MusicStore.FormUtils2


type Field = {
    Label : string
    InputF : Suave.Html.Attribute list -> Suave.Html.Xml
}

type Fieldset = {
    Legend : string
    Fields : Field list
}

type FormLayout<'a> = {
    Fieldsets : Fieldset list
    SubmitText : string
    Form : FormUtils2.Form<'a>
}

type Logon2 = {
    Username : string
    Password : string
}

let logonForm2 : Form<Logon2> = Form []
let bindLogonForm2 = bindRequest logonForm2

let logonLayout = 
    { Fieldsets = 
          [ { Legend = "Account Information"
              Fields = 
                  [ { Label = "User Name"
                      InputF = FormUtils2.textInput logonForm2 (fun f -> <@ f.Username @>) }
                    { Label = "Password"
                      InputF = FormUtils2.passwordInput logonForm2 (fun f -> <@ f.Password @>) } ] } ]
      SubmitText = "Log On"
      Form = logonForm2 }

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

type Checkout2 = {
    FirstName : string
    LastName : string
    Address : string
    PromoCode : string option
}

let checkoutForm2 : Form<Checkout2> = Form []
let bindCheckoutForm2 = bindRequest checkoutForm2

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