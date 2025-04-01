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
    
CREATE PROCEDURE sp_GetFiltered_Book_Loan_Publisher
    @PublisherName NVARCHAR(200) = NULL,
    @LoanDateFrom DATETIME = NULL,
    @LoanDateTo DATETIME = NULL,
    @IsAvailable BIT = NULL,
    @SortColumn NVARCHAR(50) = 'BookTitle', -- Default sort column
    @SortOrder NVARCHAR(4) = 'ASC', -- Default sort order
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        b.id AS BookId,
        b.title AS BookTitle,
        p.name AS PublisherName,
        l.loan_date AS LoanDate,
        b.is_available AS IsAvailable
    FROM Book b
    INNER JOIN Publisher p ON b.publisher_id = p.id
    LEFT JOIN Loan l ON b.id = l.book_id
    WHERE
        (@PublisherName IS NULL OR p.name LIKE '%' + @PublisherName + '%') AND
        (@LoanDateFrom IS NULL OR l.loan_date >= @LoanDateFrom) AND
        (@LoanDateTo IS NULL OR l.loan_date <= @LoanDateTo) AND
        (@IsAvailable IS NULL OR b.is_available = @IsAvailable)
    ORDER BY
        CASE WHEN @SortColumn = 'BookTitle' AND @SortOrder = 'ASC' THEN b.title END ASC,
        CASE WHEN @SortColumn = 'BookTitle' AND @SortOrder = 'DESC' THEN b.title END DESC,
        CASE WHEN @SortColumn = 'PublisherName' AND @SortOrder = 'ASC' THEN p.name END ASC,
        CASE WHEN @SortColumn = 'PublisherName' AND @SortOrder = 'DESC' THEN p.name END DESC,
        CASE WHEN @SortColumn = 'LoanDate' AND @SortOrder = 'ASC' THEN l.loan_date END ASC,
        CASE WHEN @SortColumn = 'LoanDate' AND @SortOrder = 'DESC' THEN l.loan_date END DESC,
        CASE WHEN @SortColumn = 'IsAvailable' AND @SortOrder = 'ASC' THEN b.is_available END ASC,
        CASE WHEN @SortColumn = 'IsAvailable' AND @SortOrder = 'DESC' THEN b.is_available END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

END;

GO
CREATE PROCEDURE get_Xml
    @PublisherName NVARCHAR(200) = NULL,
    @LoanDateFrom DATETIME = NULL,
    @LoanDateTo DATETIME = NULL,
    @IsAvailable BIT = NULL,
    @SortColumn NVARCHAR(50) = 'BookTitle',
    @SortOrder NVARCHAR(4) = 'ASC'
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        b.id AS BookId,
        b.title AS BookTitle,
        p.name AS PublisherName,
        l.loan_date AS LoanDate,
        b.is_available AS IsAvailable
    FROM Book b
    INNER JOIN Publisher p ON b.publisher_id = p.id
    LEFT JOIN Loan l ON b.id = l.book_id
    WHERE
        (@PublisherName IS NULL OR p.name LIKE '%' + @PublisherName + '%') AND
        (@LoanDateFrom IS NULL OR l.loan_date >= @LoanDateFrom) AND
        (@LoanDateTo IS NULL OR l.loan_date <= @LoanDateTo) AND
        (@IsAvailable IS NULL OR b.is_available = @IsAvailable)
    ORDER BY
        CASE WHEN @SortColumn = 'BookTitle' AND @SortOrder = 'ASC' THEN b.title END ASC,
        CASE WHEN @SortColumn = 'BookTitle' AND @SortOrder = 'DESC' THEN b.title END DESC,
        CASE WHEN @SortColumn = 'PublisherName' AND @SortOrder = 'ASC' THEN p.name END ASC,
        CASE WHEN @SortColumn = 'PublisherName' AND @SortOrder = 'DESC' THEN p.name END DESC,
        CASE WHEN @SortColumn = 'LoanDate' AND @SortOrder = 'ASC' THEN l.loan_date END ASC,
        CASE WHEN @SortColumn = 'LoanDate' AND @SortOrder = 'DESC' THEN l.loan_date END DESC,
        CASE WHEN @SortColumn = 'IsAvailable' AND @SortOrder = 'ASC' THEN b.is_available END ASC,
        CASE WHEN @SortColumn = 'IsAvailable' AND @SortOrder = 'DESC' THEN b.is_available END DESC
    FOR XML PATH('BookLoan'), ROOT('BookLoanData');
END;

GO
CREATE PROCEDURE get_Json
    @PublisherName NVARCHAR(200) = NULL,
    @LoanDateFrom DATETIME = NULL,
    @LoanDateTo DATETIME = NULL,
    @IsAvailable BIT = NULL,
    @SortColumn NVARCHAR(50) = 'BookTitle',
    @SortOrder NVARCHAR(4) = 'ASC'
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        (SELECT
            b.id AS BookId,
            b.title AS BookTitle,
            p.name AS PublisherName,
            l.loan_date AS LoanDate,
            b.is_available AS IsAvailable
        FROM Book b
        INNER JOIN Publisher p ON b.publisher_id = p.id
        LEFT JOIN Loan l ON b.id = l.book_id
        WHERE
            (@PublisherName IS NULL OR p.name LIKE '%' + @PublisherName + '%') AND
            (@LoanDateFrom IS NULL OR l.loan_date >= @LoanDateFrom) AND
            (@LoanDateTo IS NULL OR l.loan_date <= @LoanDateTo) AND
            (@IsAvailable IS NULL OR b.is_available = @IsAvailable)
        ORDER BY
            CASE WHEN @SortColumn = 'BookTitle' AND @SortOrder = 'ASC' THEN b.title END ASC,
            CASE WHEN @SortColumn = 'BookTitle' AND @SortOrder = 'DESC' THEN b.title END DESC,
            CASE WHEN @SortColumn = 'PublisherName' AND @SortOrder = 'ASC' THEN p.name END ASC,
            CASE WHEN @SortColumn = 'PublisherName' AND @SortOrder = 'DESC' THEN p.name END DESC,
            CASE WHEN @SortColumn = 'LoanDate' AND @SortOrder = 'ASC' THEN l.loan_date END ASC,
            CASE WHEN @SortColumn = 'LoanDate' AND @SortOrder = 'DESC' THEN l.loan_date END DESC,
            CASE WHEN @SortColumn = 'IsAvailable' AND @SortOrder = 'ASC' THEN b.is_available END ASC,
            CASE WHEN @SortColumn = 'IsAvailable' AND @SortOrder = 'DESC' THEN b.is_available END DESC
        FOR JSON PATH) AS JsonData;
END;

GO
CREATE TRIGGER tr_LoanDurationCheck
ON Loan
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM inserted
        WHERE DATEDIFF(day, loan_date, due_date) > 30
    )
    BEGIN
        RAISERROR('Loan duration exceeds 30 days.', 16, 1);
        ROLLBACK TRANSACTION;
    END;
END;
