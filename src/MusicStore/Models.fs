module MusicStore.Models

type AlbumBrief = {
    Id : int
    Title : string
}

type Genre = {
    Name : string
    Albums : AlbumBrief []
}

type Store = {
    Genres : string []
}

type Album = {
    Id : int 
    Title : string
    Artist : string
    Genre : string
    Price : decimal
}

type Manage = {
    Albums : Album []
}