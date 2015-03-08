module MusicStore.Domain

type CreateAlbumCommand = {
    Title : string
    GenreId : int
    ArtistId : int
    ArtUrl : string
    Price : decimal
}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module CreateAlbumCommand =
    let create (artistId, genreId, title, price, artUrl) = 
        { CreateAlbumCommand.ArtistId = artistId
          GenreId = genreId
          Title = title
          Price = price
          ArtUrl = artUrl }

type UpdateAlbumCommand = {
    Id : int
    Title : string
    GenreId : int
    ArtistId : int
    ArtUrl : string
    Price : decimal
}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module UpdateAlbumCommand =
    let create albumId (artistId, genreId, title, price, artUrl) = 
        { UpdateAlbumCommand.Id = albumId
          ArtistId = artistId
          GenreId = genreId
          Title = title
          Price = price
          ArtUrl = artUrl }

type DeleteAlbumCommand = int