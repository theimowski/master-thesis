module MusicStore.View

open DotLiquid

type Index = {
    Container : string
    Genres : string[]
}

type Home = { 
    Placeholder : unit
}

type Store = {
    Genres : string []
}

type Album = {
    Id : int 
    Title : string
    Artist : string
    Genre : string
    Price : string
    Art : string
}

type DeleteAlbum = {
    Id : int
    Title : string
}

type Genre = {
    Name : string
    Albums : (int * string) [] 
}

type ManageStore = {
    Albums : Album []
}

type CreateAlbum = {
    Artists : (int * string) []
    Genres : (int * string) []
}

type EditAlbum = {
    Artists : (int * string) []
    Genres : (int * string) []
    Album : Album
}

let templates =
    Template.RegisterSafeType(typeof<int * string>, [|"Item1";"Item2"|])

    let registerTemplate modelType = 
        let fields = Reflection.FSharpType.GetRecordFields(modelType) |> Array.map (fun f -> f.Name)
        Template.RegisterSafeType(modelType, fields)
        modelType, Template.Parse(System.IO.File.ReadAllText(modelType.Name + ".html"))

    typeof<Album>.DeclaringType.GetNestedTypes()
    |> Array.map registerTemplate
    |> dict

let render model =  
    let template = templates.[model.GetType()]
    template.Render(DotLiquid.Hash.FromAnonymousObject(model))