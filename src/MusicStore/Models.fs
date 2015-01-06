module MusicStore.Models

type Album = {
    Id : int
    Title : string
}

type Genre = {
    Name : string
    Albums : Album []
}

type Store = {
    Genres : string []
}