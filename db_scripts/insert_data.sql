insert into Publisher(name, place) values
	('Penguin Books', 'London'),
	('Random House', 'New York'),
    ('HarperCollins', 'New York'),
    ('Macmillan Publishers', 'London'),
    ('Scholastic', 'Chicago'),
    ('Hachette Livre', 'Paris'),
    ('Simon & Schuster', 'Boston'),
    ('Wiley', 'Hoboken'),
    ('Bloomsbury', 'London'),
	('Oxford University Press','Oxford');

insert into Student(
	name,
	email,
	phone_number,
	address_street,
	address_city,
	address_zipcode) values
	('00017747', '00017747@wiut.uz', '998-90-033-69-09', '5 Uzar St', 'Tashkent', '100149'),
    ('Topson', 'topson@example.com', '234-567-8901', '456 Oak Ave', 'Helsinki', '10001'),
    ('Ana', 'ana@example.com', '345-678-9012', '789 Pine Rd', 'Sydney', '60601'),
    ('Ceb', 'ceb@example.com', '456-789-0123', '321 Elm St', 'Paris', '33101'),
    ('Jerax', 'jerax@example.com', '567-890-1234', '654 Birch Ln', 'Helsinki', '98101'),
    ('Notail', 'notail@example.com', '678-901-2345', '987 Cedar Dr', 'Copenhagen', '73301'),
    ('Gorgc', 'gorgc@example.com', '789-012-3456', '147 Maple Ave', 'Stockholm', '80201'),
    ('ODPixel', 'ODPixel@example.com', '890-123-4567', '258 Willow Rd', 'London', '97201'),
    ('Fogged', 'fog@example.com', '901-234-5678', '369 Spruce St', 'Washingtion', '85001'),
    ('Cap', 'cap@example.com', '012-345-6789', '741 Ash Blvd', 'San Diego', '92101');

insert into Book (
	title,
	physical_location,
	is_available,
	isbn,
	publisher_id,
	cover_image,
	language,
	publication_date
) values
    ('The Great Gatsby', 'Silent Area', 1, '9780743273565', 1, null, 'English', '1925'),
    ('1984', 'Main Area', 0, '9780451524935', 2, null, 'English', '1949'),
    ('To Kill a Mockingbird', 'Silent Area', 1, '9780446310789', 3, null, 'English', '1960'),
    ('Pride and Prejudice', 'Silent Area', 1, '9780141439518', 4, null, 'English', '1813'),
    ('The Catcher in the Rye', 'Silent Area', 1, '9780316769488', 5, null, 'English', '1951'),
    ('Animal Farm', 'Main Area', 0, '9780451526342', 6, null, 'English', '1945'),
    ('Lord of the Rings', 'Main Area', 1, '9780618640157', 7, null, 'English', '1954'),
    ('Brave New World', 'Silent Area', 1, '9780060850524', 8, null, 'English', '1932'),
    ('The Hobbit', 'Main Area', 0, '9780547928227', 9, null, 'English', '1937'),
    ('Fahrenheit 451', 'Silent Area', 1, '9781451673319', 10, null, 'English', '1953');

    insert into Loan (loan_date, due_date, return_date, student_id, book_id) values
    ('2025-03-01', '2025-03-15', NULL, 1, 1),
    ('2025-03-02', '2025-03-16', '2025-03-10', 2, 2),
    ('2025-03-03', '2025-03-17', NULL, 3, 3),
    ('2025-03-04', '2025-03-18', NULL, 4, 4),
    ('2025-03-05', '2025-03-19', '2025-03-12', 5, 5),
    ('2025-03-06', '2025-03-20', NULL, 6, 6),
    ('2025-03-07', '2025-03-21', NULL, 7, 7),
    ('2025-03-08', '2025-03-22', '2025-03-15', 8, 8),
    ('2025-03-09', '2025-03-23', NULL, 9, 9),
    ('2025-03-10', '2025-03-24', NULL, 10, 10);