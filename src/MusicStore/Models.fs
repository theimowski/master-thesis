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
    Price : string
    Art : string
}

type Manage = {
    Albums : Album []
}

type ArtistBrief = {
    Id : int
    Name : string
}

type GenreBrief = {
    Id : int
    Name : string
}

type CreateAlbum = {
    Artists : ArtistBrief []
    Genres : GenreBrief []
}

type EditAlbum = {
    Artists : ArtistBrief []
    Genres : GenreBrief []
    Album : Album
}

type Index = {
    Container : string
    Genres : string[]
}