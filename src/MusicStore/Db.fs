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
type AlbumDetails = DbContext.``[dbo].[AlbumDetails]Entity``
type Cart = DbContext.``[dbo].[Carts]Entity``
type CartDetails = DbContext.``[dbo].[CartDetails]Entity``
type Order = DbContext.``[dbo].[Orders]Entity``
type OrderDetails = DbContext.``[dbo].[OrderDetails]Entity``
type User = DbContext.``[dbo].[Users]Entity``
type BestSeller = DbContext.``[dbo].[BestSellers]Entity``

let firstOrNone s = s |> Seq.tryFind (fun _ -> true)

let getAlbum id (ctx : DbContext) : Album option = 
    query { 
        for album in ctx.``[dbo].[Albums]`` do
            where (album.AlbumId = id)
            select album
    } |> firstOrNone

let getBestSellers (ctx : DbContext) : BestSeller list  =
    ctx.``[dbo].[BestSellers]`` |> Seq.toList

let getAlbumsDetails (ctx : DbContext) : AlbumDetails list = 
    ctx.``[dbo].[AlbumDetails]`` |> Seq.toList

let getAlbumDetails id (ctx : DbContext) : AlbumDetails option = 
    query { 
        for album in ctx.``[dbo].[AlbumDetails]`` do
            where (album.AlbumId = id)
            select album
    } |> firstOrNone

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
    } |> firstOrNone

let getGenres (ctx : DbContext) : Genre list = 
    ctx.``[dbo].[Genres]`` |> Seq.toList
    
let getArtists (ctx : DbContext) : Artist list = 
    ctx.``[dbo].[Artists]`` |> Seq.toList

let newAlbum (ctx : DbContext) : Album =
    ctx.``[dbo].[Albums]``.Create()

let getCart cartId albumId (ctx : DbContext) : Cart option =
    query {
        for cart in ctx.``[dbo].[Carts]`` do
            where (cart.CartId = cartId && cart.AlbumId = albumId)
            select cart
    } |> firstOrNone

let getCarts cartId (ctx : DbContext) : Cart list =
    ctx.``[dbo].[Carts]`` |> Seq.toList

let getCartsDetails cartId (ctx : DbContext) : CartDetails list =
    query {
        for cart in ctx.``[dbo].[CartDetails]`` do
            where (cart.CartId = cartId)
            select cart
    } |> Seq.toList

let newOrder total username (ctx : DbContext) : Order =
    let order = ctx.``[dbo].[Orders]``.Create(DateTime.UtcNow, total)
    order.Username <- username
    order

let newOrderDetails (albumId, orderId, quantity, unitPrice) (ctx : DbContext) : OrderDetails =
    ctx.``[dbo].[OrderDetails]``.Create(albumId, orderId, quantity, unitPrice)

let newUser (email, password, role, username) (ctx : DbContext) : User =
    ctx.``[dbo].[Users]``.Create(email, password, role, username)

let validateUser (username, password) (ctx : DbContext) : User option =
    query {
        for user in ctx.``[dbo].[Users]`` do
            where (user.UserName = username && user.Password = password)
            select user
    } |> firstOrNone

let addToCart cartId albumId (ctx : DbContext)  =
    match getCart cartId albumId ctx with
    | Some cart ->
        cart.Count <- cart.Count + 1
    | None ->
        ctx.``[dbo].[Carts]``.Create(albumId, cartId, 1, DateTime.UtcNow) |> ignore
    ctx.SubmitUpdates()

let removeFromCart (cart : Cart) albumId (ctx : DbContext) = 
    cart.Count <- cart.Count - 1
    if cart.Count = 0 then cart.Delete()
    ctx.SubmitUpdates()

let placeOrder (username : string) (ctx : DbContext) =
    let carts = getCartsDetails username ctx
    let total = carts |> List.sumBy (fun c -> (decimal) c.Count * c.Price)
    let order = newOrder total username ctx
    ctx.SubmitUpdates()
    for cart in carts do
        let orderDetails = newOrderDetails (cart.AlbumId, order.OrderId, cart.Count, cart.Price) ctx
        getCart cart.CartId cart.AlbumId ctx
        |> Option.iter (fun cart -> cart.Delete())
    ctx.SubmitUpdates()
    order