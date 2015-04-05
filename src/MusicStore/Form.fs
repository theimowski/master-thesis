module MusicStore.Form

open MusicStore.FormUtils

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
    let Password = TextField("password", [])
    let ConfirmPassword = TextField("confirmpassword", [])

    let passwordsMatch result =
        result.GetText Password = result.GetText ConfirmPassword, "Passwords must match"
    
    let form = {
        Fields = [TextFormField Username; TextFormField Email; TextFormField Password; TextFormField ConfirmPassword]
        ServerSideValidations = [ passwordsMatch ]
    }

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