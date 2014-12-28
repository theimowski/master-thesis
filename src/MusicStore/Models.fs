module MusicStore.Models

type Album = {
    Title : string
}

type Genre = {
    Name : string
}

type Store = {
    Genres : Genre []
}