create table Users 
(
UserId int primary key identity(1,1),
UserName nvarchar(200) not null,
Email nvarchar(120) not null,
Password nvarchar(200) not null,
Role nvarchar(50) not null
)