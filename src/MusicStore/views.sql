create view AlbumDetails 
as
select a.AlbumId, a.AlbumArtUrl, a.Price, a.Title, at.Name as Artist, g.Name as Genre
from Albums a
join Artists at on at.ArtistId = a.ArtistId
join Genres g on g.GenreId = a.GenreId

go

create view CartDetails
as
select c.CartId, c.Count, a.Title as AlbumTitle, a.AlbumId as AlbumId, a.Price
from Carts c
join Albums a on c.AlbumId = a.AlbumId