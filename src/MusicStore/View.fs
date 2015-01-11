module MusicStore.View

open System

open MusicStore.Domain

open DotLiquid

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