module MusicStore.Db

open System
open System.Data
open System.Linq

open FSharp.Data.Sql

type sql = 
    SqlDataProvider< 
        "Server=(LocalDb)\\v11.0;Database=MvcMusicStore;Trusted_Connection=True;MultipleActiveResultSets=true", 
        DatabaseVendor=Common.DatabaseProviderTypes.MSSQLSERVER >

type DbContext = sql.dataContext
type Album = DbContext.``[dbo].[Albums]Entity``
type Genre = DbContext.``[dbo].[Genres]Entity``
type Artist = DbContext.``[dbo].[Artists]Entity``
type AlbumDetails = Album * Artist * Genre

let (|Album|) ((a,_,_) : AlbumDetails) = a
let (|Artist|) ((_,a,_) : AlbumDetails) = a
let (|Genre|) ((_,_,g) : AlbumDetails) = g

let getAlbum id (ctx : DbContext) : Album = 
    query { 
        for album in ctx.``[dbo].[Albums]`` do
            where (album.AlbumId = id)
            select album
            exactlyOne
    }

let getAlbumsDetails (ctx : DbContext) : AlbumDetails list = 
    query { 
        for album in ctx.``[dbo].[Albums]`` do
            join artist in ctx.``[dbo].[Artists]`` on (album.ArtistId = artist.ArtistId)
            join genre in ctx.``[dbo].[Genres]`` on (album.GenreId = genre.GenreId)
            select (album, artist, genre)
    }
    |> Seq.toList

let getAlbumDetails id (ctx : DbContext) : AlbumDetails = 
    query { 
        for album in ctx.``[dbo].[Albums]`` do
            where (album.AlbumId = id)
            join artist in ctx.``[dbo].[Artists]`` on (album.ArtistId = artist.ArtistId)
            join genre in ctx.``[dbo].[Genres]`` on (album.GenreId = genre.GenreId)
            select (album, artist, genre)
            exactlyOne
    }

let getAlbumsForGenre genreId (ctx : DbContext) : Album list = 
    query { 
        for album in ctx.``[dbo].[Albums]`` do
            where (album.GenreId = genreId)
            select album
    }
    |> Seq.toList

let getGenre name (ctx : DbContext) : Genre = 
    query { 
        for genre in ctx.``[dbo].[Genres]`` do
            where (genre.Name = name)
            select genre
            exactlyOne
    }

let getGenres (ctx : DbContext) : Genre list = 
    ctx.``[dbo].[Genres]`` |> Seq.toList
    
let getArtists (ctx : DbContext) : Artist list = 
    ctx.``[dbo].[Artists]`` |> Seq.toList

let newAlbum (ctx : DbContext) : Album =
    ctx.``[dbo].[Albums]``.Create()

let saveAlbum albumF setterF (ctx : DbContext) =
    setterF (albumF ctx)
    ctx.SubmitUpdates()

let deleteAlbum id (ctx : DbContext) = 
    let album = getAlbum id ctx
    album.Delete()
    ctx.SubmitUpdates()