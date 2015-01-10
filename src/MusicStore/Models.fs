module MusicStore.Models

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