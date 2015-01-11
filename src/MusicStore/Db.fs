module MusicStore.Db

open System
open System.Data
open System.Linq

open MusicStore.Domain
open MusicStore.View

open FSharp.Data.Sql

type sql = SqlDataProvider< 
              "Server=(LocalDb)\\v11.0;Database=MvcMusicStore;Trusted_Connection=True;MultipleActiveResultSets=true",
              DatabaseVendor = Common.DatabaseProviderTypes.MSSQLSERVER>

type DbContext = sql.dataContext

let getHome (_) =   
    {Placeholder = ()}

let getStore (dbContext : DbContext) = 
    let genres = 
        query {
            for g in dbContext.``[dbo].[Genres]`` do
            select (Uri.EscapeDataString g.Name)
        } 
        |> Seq.toArray

    {Store.Genres = genres}

let getGenre name (ctx : DbContext) = 
    let albums = 
        query {
            for a in ctx.``[dbo].[Albums]`` do
            where (a.GenreId = query {
                for g in ctx.``[dbo].[Genres]`` do 
                where (g.Name = name)
                select g.GenreId
                exactlyOne
            })
            select (a.AlbumId, a.Title)
        } |> Seq.toArray

    {Name = name; Albums = albums}

let getAlbum id (ctx : DbContext) =
    query {
        for album in ctx.``[dbo].[Albums]`` do 
        where (album.AlbumId = id)
        join artist in ctx.``[dbo].[Artists]`` on (album.ArtistId = artist.ArtistId)
        join genre in ctx.``[dbo].[Genres]`` on (album.GenreId = genre.GenreId)
        select { 
            Album.Id = album.AlbumId
            Title = album.Title
            Artist = artist.Name
            Genre = genre.Name
            Price = album.Price.ToString(Globalization.CultureInfo.InvariantCulture)
            Art = album.AlbumArtUrl
        }
        exactlyOne
    }

let getManageStore (ctx : DbContext) =
    let albums = 
        query {
            for album in ctx.``[dbo].[Albums]`` do 
            join artist in ctx.``[dbo].[Artists]`` on (album.ArtistId = artist.ArtistId)
            join genre in ctx.``[dbo].[Genres]`` on (album.GenreId = genre.GenreId)
            select { 
                Album.Id = album.AlbumId
                Title = album.Title
                Artist = artist.Name
                Genre = genre.Name
                Price = album.Price.ToString(Globalization.CultureInfo.InvariantCulture)
                Art = album.AlbumArtUrl
            }
        } |> Seq.toArray

    {Albums = albums}

let getCreateAlbum (ctx : DbContext) =
    let genres = 
        query {
            for g in ctx.``[dbo].[Genres]`` do select (g.GenreId, g.Name)
        } |> Seq.toArray

    let artists = 
        query {
            for a in ctx.``[dbo].[Artists]`` do select (a.ArtistId, a.Name)
        } |> Seq.toArray

    {CreateAlbum.Genres = genres; Artists = artists}

let getEditAlbum id (ctx : DbContext) =
    let genres = 
        query {
            for g in ctx.``[dbo].[Genres]`` do select (g.GenreId, g.Name)
        } |> Seq.toArray

    let artists = 
        query {
            for a in ctx.``[dbo].[Artists]`` do select (a.ArtistId,a.Name)
        } |> Seq.toArray

    let album = 
        query {
            for a in ctx.``[dbo].[Albums]`` do
            where (a.AlbumId = id)
            select {
                Album.Id = a.AlbumId
                Title = a.Title
                Price = a.Price.ToString(Globalization.CultureInfo.InvariantCulture)
                Art = a.AlbumArtUrl
                Artist = artists |> Seq.find (fun (id,_) -> id = a.ArtistId) |> snd
                Genre = genres |> Seq.find (fun (id,_) -> id = a.GenreId) |> snd
            }
            exactlyOne
        }

    {EditAlbum.Genres = genres; Artists = artists; Album = album}

let getDeleteAlbum id (ctx : DbContext) =
    let title = query {
        for a in ctx.``[dbo].[Albums]`` do
        where (a.AlbumId = id)
        select a.Title
        exactlyOne
    }

    {DeleteAlbum.Id = id; Title = title}

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
    let album = query {
        for a in ctx.``[dbo].[Albums]`` do
        where (a.AlbumId = c)
        select a
        exactlyOne
    }
        
    album.Delete()
    ctx.SubmitUpdates()