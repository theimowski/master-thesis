create view AlbumDetails 
as
select a.AlbumId, a.AlbumArtUrl, a.Price, a.Title, at.Name as Artist, g.Name as Genre
from Albums a
join Artists at on at.ArtistId = a.ArtistId
join Genres g on g.GenreId = a.GenreId