module MusicStore.Form

open System

open Suave.Model

let bindForm key = Binding.form key Choice1Of2

let passHash (pass: string) =
    use sha = Security.Cryptography.SHA256.Create()
    Text.Encoding.UTF8.GetBytes(pass)
    |> sha.ComputeHash
    |> Array.map (fun b -> b.ToString("x2"))
    |> String.concat ""

let albumForm req = 
    binding {
        let! artistId = req |> bindForm "artist" >>. Parse.int32
        let! genreId = req |> bindForm "genre" >>. Parse.int32
        let! title = req |> bindForm "title"
        let! price = req |> bindForm "price" >>. Parse.decimal
        let! artUrl = req |> bindForm "artUrl"

        return (fun (album : Db.Album) -> 
            album.ArtistId <- artistId
            album.GenreId <- genreId
            album.Title <- title
            album.Price <- price
            album.AlbumArtUrl <- artUrl
        )
    }

let registerForm request =
    binding {
        let! username = request |> bindForm "username"
        let! email = request |> bindForm "email"
        let! password = request |> bindForm "password"
        let matchesPassword = function 
        | p when p = password -> Choice1Of2 password 
        | _ -> Choice2Of2 "Passwords do not match"
        let! confirmpassword = request |> bindForm "confirmpassword" >>. matchesPassword

        return (fun (user : Db.User) ->
            user.UserName <- username
            user.Email <- email
            user.Password <- passHash password
        )
    }

let logonForm req =
    binding {
        let! username = req |> bindForm "username"
        let! password = req |> bindForm "password"

        return username,password
    }