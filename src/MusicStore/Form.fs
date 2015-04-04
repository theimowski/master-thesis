module MusicStore.Form

open MusicStore.FormUtils

module Logon = 
    let Username = TextFieldName("username")
    let Password = TextFieldName("password")
    
    let form = 
        Form([ TextField(Username, 
                         [ MinLength 5
                           MaxLength 20 ])
               TextField(Password, []) ], [])

let logonForm req = bindingForm Logon.form req

module Register =
    let Username = TextFieldName("username")
    let Email = TextFieldName("email")
    let Password = TextFieldName("password")
    let ConfirmPassword = TextFieldName("confirmpassword")

    let passwordsMatch result =
        result.GetText Password = result.GetText ConfirmPassword, "Passwords must match"
    
    let form = 
        Form ([ TextField(Username, [])
                TextField(Email, [])
                TextField(Password, [])
                TextField(ConfirmPassword, []) ], [ passwordsMatch ])


let registerForm req = bindingForm Register.form req

module Album =
    let ArtistId = IntegerFieldName "artist"
    let GenreId = IntegerFieldName "genre"
    let Title = TextFieldName "title"
    let Price = DecimalFieldName "price"
    let ArtUrl = TextFieldName "artUrl"

    let form = 
        Form ([ IntegerField(ArtistId, [])
                IntegerField(GenreId, [])
                TextField(Title, [])
                DecimalField(Price, []) 
                TextField(ArtUrl, []) ], [ ])

let albumForm req = bindingForm Album.form req