create procedure sp_Create_Book
	@title nvarchar(200),
	@physical_location nvarchar(200),
	@is_available BIT,
	@isbn nvarchar(13),
	@publisher_id int,
	@cover_image varbinary(max),
	@language nvarchar(20),
	@publication_date datetime
as 
begin
	insert into Book (title, physical_location, is_available, isbn, publisher_id, cover_image, language, publication_date)
	values (@title, @physical_location, @is_available, @isbn, @publisher_id, @cover_image, @language, @publication_date);
	select SCOPE_IDENTITY() as NewBookId
end
go

create procedure sp_Get_All_Books
as 
begin
	select * from Book
end
go

create procedure sp_Get_Book_By_Id
	@id int
as
begin
	select * from Book where id = @id
end
go

create procedure sp_Update_Book
	@id int,
	@title nvarchar(200),
    @physical_location nvarchar(100),
    @is_available bit,
    @isbn nvarchar(13),
    @publisher_id int,
    @cover_image varbinary(max),
	@language nvarchar(20),
    @publication_date datetime
as 
begin
	update Book
	set title = @title,
		physical_location = @physical_location,
		is_available = @is_available,
		isbn = @isbn,
		publisher_id = @publisher_id,
		cover_image = @cover_image,
		language = @language,
		publication_date = @publication_date
	where id = @id

end
go

create procedure sp_Delete_Book
	@id int
as 
begin 
	delete from Book where id= @id
end
go
    



