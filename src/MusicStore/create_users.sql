create table Users 
(
UserId int primary key identity(1,1),
UserName nvarchar(200) not null,
Email nvarchar(120) not null,
Password nvarchar(200) not null,
Role nvarchar(50) not null
)

insert into Users (UserName, Email, Password, Role)
values ('admin', 'admin@example.com', 
-- hash for 'admin' password
'8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 
'admin')