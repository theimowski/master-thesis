module MusicStore.Db

open System
open System.Data
open System.Linq

open FSharp.Data.Sql

type sql = 
    SqlDataProvider< 
        "Server=localhost;Database=MvcMusicStore;Trusted_Connection=True;MultipleActiveResultSets=true", 
        DatabaseVendor=Common.DatabaseProviderTypes.MSSQLSERVER >

type DbContext = sql.dataContext
type Album = DbContext.``[dbo].[Albums]Entity``
type Genre = DbContext.``[dbo].[Genres]Entity``
type Artist = DbContext.``[dbo].[Artists]Entity``
type AlbumDetails = DbContext.``[dbo].[AlbumDetails]Entity``

let option = function | null -> None | x -> Some x

let getAlbum (ctx : DbContext) id : Album option = 
    query { 
        for album in ctx.``[dbo].[Albums]`` do
            where (album.AlbumId = id)
            select album
            exactlyOneOrDefault
    } |> option

let getAlbumsDetails (ctx : DbContext) : AlbumDetails list = 
    ctx.``[dbo].[AlbumDetails]`` |> Seq.toList

let getAlbumDetails (ctx : DbContext) id : AlbumDetails option = 
    query { 
        for album in ctx.``[dbo].[AlbumDetails]`` do
            where (album.AlbumId = id)
            select album
            exactlyOneOrDefault
    } |> option

let getAlbumsForGenre genreId (ctx : DbContext) : Album list = 
    query { 
        for album in ctx.``[dbo].[Albums]`` do
            where (album.GenreId = genreId)
            select album
    }
    |> Seq.toList

let getGenre name (ctx : DbContext) : Genre option = 
    query { 
        for genre in ctx.``[dbo].[Genres]`` do
            where (genre.Name = name)
            select genre
            exactlyOneOrDefault
    } |> option

let getGenres (ctx : DbContext) : Genre list = 
    ctx.``[dbo].[Genres]`` |> Seq.toList
    
let getArtists (ctx : DbContext) : Artist list = 
    ctx.``[dbo].[Artists]`` |> Seq.toList

let newAlbum (ctx : DbContext) : Album =
    ctx.``[dbo].[Albums]``.Create()

let saveAlbum (ctx : DbContext) albumF setterF =
    setterF (albumF ())
    ctx.SubmitUpdates()

let deleteAlbum (ctx : DbContext) id = 
    match getAlbum ctx id with
    | Some album ->
        album.Delete()
        ctx.SubmitUpdates()
        Some ()
    | None -> None