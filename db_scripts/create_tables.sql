
create table Publisher (
	id int primary key identity(1,1),
	name nvarchar(200) not null,
	place nvarchar(100) not null,
)

create table Student (
	id int primary key identity(1,1),
	name nvarchar(100) not null,
	email nvarchar(100) not null,
	phone_number nvarchar(20) not null,
	address_street nvarchar(100) not null,
	address_city nvarchar(100) not null,
	address_zipcode nvarchar(100) not null,
)

create table Book (
	id int primary key identity(1,1),
	title nvarchar(200) not null,
	physical_location nvarchar(100),
	is_available bit not null default 1,
	isbn nvarchar(13) unique,
	publisher_id int not null,
	cover_image varbinary(max),
	language nvarchar(20) not null,
	publication_date datetime
	foreign key (publisher_id) references Publisher(id)
)

create table Loan (
	id int primary key identity(1,1),
	loan_date datetime not null,
	due_date datetime not null,
	return_date datetime,
	student_id int not null,
	book_id int not null,
	foreign key (student_id) references Student(id),
	foreign key (book_id) references Book(id)
)
