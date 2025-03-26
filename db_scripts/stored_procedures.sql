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
    
create procedure GetFilteredPagedSortedBooks
    @TitleFilter nvarchar(200) = null,
    @PublicationDateFilter datetime = null,
    @PublisherNameFilter nvarchar(200) = null,
    @SortColumn nvarchar(50) = 'title',
    @SortDirection nvarchar(4) = 'ASC',
    @PageNumber int = 1,
    @PageSize int = 10
as
begin
    set nocount on;

    -- Validate sort column and direction
    if @SortColumn not in ('title', 'publication_date', 'publisher_name')
        set @SortColumn = 'title';
    if @SortDirection not in ('ASC', 'DESC')
        set @SortDirection = 'ASC';

    declare @Offset int = (@PageNumber - 1) * @PageSize;

    select
        b.Id as BookId,
        b.Title,
        b.Publication_Date,
        p.Name as PublisherName,
        count(*) over() as TotalRecords
    from
        Book b
    inner join
        Publisher p on b.publisher_id = p.id
    left join
        Loan l on b.id = l.book_id
    where
        (@TitleFilter is null or b.Title LIKE '%' + @TitleFilter + '%') and
        (@PublicationDateFilter is null or b.Publication_Date >= @PublicationDateFilter) and
        (@PublisherNameFilter is null or p.Name LIKE '%' + @PublisherNameFilter + '%')
    order by
        case @SortColumn
            when 'title' then b.Title
            when 'publication_date' then b.Publication_Date
            when 'publisher_name' then p.Name
        end desc
    offset @Offset rows
    fetch next @PageSize rows only;
end



