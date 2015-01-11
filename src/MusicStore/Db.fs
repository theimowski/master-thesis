module MusicStore.Db

open System
open System.Data
open System.Linq

open MusicStore.Domain

open FSharp.Data.Sql

type sql = 
    SqlDataProvider< 
        "Server=(LocalDb)\\v11.0;Database=MvcMusicStore;Trusted_Connection=True;MultipleActiveResultSets=true", 
        DatabaseVendor=Common.DatabaseProviderTypes.MSSQLSERVER >

type DbContext = sql.dataContext

let getAlbums (ctx : DbContext) = 
    query { 
        for album in ctx.``[dbo].[Albums]`` do
            join artist in ctx.``[dbo].[Artists]`` on (album.ArtistId = artist.ArtistId)
            join genre in ctx.``[dbo].[Genres]`` on (album.GenreId = genre.GenreId)
            select { Album.Id = album.AlbumId
                     Title = album.Title
                     Artist = artist.Name
                     Genre = genre.Name
                     Price = album.Price.ToString(Globalization.CultureInfo.InvariantCulture)
                     Art = album.AlbumArtUrl }
    }
    |> Seq.toArray

let getAlbum id (ctx : DbContext) = 
    query { 
        for album in ctx.``[dbo].[Albums]`` do
            where (album.AlbumId = id)
            join artist in ctx.``[dbo].[Artists]`` on (album.ArtistId = artist.ArtistId)
            join genre in ctx.``[dbo].[Genres]`` on (album.GenreId = genre.GenreId)
            select { Album.Id = album.AlbumId
                     Title = album.Title
                     Artist = artist.Name
                     Genre = genre.Name
                     Price = album.Price.ToString(Globalization.CultureInfo.InvariantCulture)
                     Art = album.AlbumArtUrl }
            exactlyOne
    }

let getAlbumsForGenre genreId (ctx : DbContext) = 
    query { 
        for album in ctx.``[dbo].[Albums]`` do
            where (album.GenreId = genreId)
            select { IdAndName.Id = album.AlbumId
                     Name = album.Title }
    }
    |> Seq.toArray

let getGenre name (ctx : DbContext) = 
    query { 
        for g in ctx.``[dbo].[Genres]`` do
            where (g.Name = name)
            select { Genre.Id = g.GenreId
                     Name = g.Name }
            exactlyOne
    }

let getGenres (ctx : DbContext) = 
    query { 
        for g in ctx.``[dbo].[Genres]`` do
            select { Genre.Id = g.GenreId
                     Name = g.Name }
    }
    |> Seq.toArray

let getArtists (ctx : DbContext) = 
    query { 
        for a in ctx.``[dbo].[Artists]`` do
            select { Artist.Id = a.ArtistId
                     Name = a.Name }
    }
    |> Seq.toArray


let createAlbum (c : CreateAlbumCommand) (ctx : DbContext) = 
    let album = ctx.``[dbo].[Albums]``.Create(c.ArtistId, c.GenreId, c.Price, c.Title)
    album.AlbumArtUrl <- c.ArtUrl
    ctx.SubmitUpdates()

let updateAlbum (c : UpdateAlbumCommand) (ctx : DbContext) = 
    let album = 
        query { 
            for a in ctx.``[dbo].[Albums]`` do
                where (a.AlbumId = c.Id)
                select a
                exactlyOne
        }
    album.ArtistId <- c.ArtistId
    album.GenreId <- c.GenreId
    album.Title <- c.Title
    album.Price <- c.Price
    album.AlbumArtUrl <- c.ArtUrl
    ctx.SubmitUpdates()

let deleteAlbum (c : DeleteAlbumCommand) (ctx : DbContext) = 
    let album = 
        query { 
            for a in ctx.``[dbo].[Albums]`` do
                where (a.AlbumId = c)
                select a
                exactlyOne
        }
    album.Delete()
    ctx.SubmitUpdates()