-- CREATE DATABASE
CREATE DATABASE [st10385722-agri-energy-connect-db];

-- Firstly, we will create the user roles table, which houses all roles of the website
CREATE TABLE USER_ROLE(
	role_id INT PRIMARY KEY,
	role_name VARCHAR(MAX) NOT NULL,
	role_description VARCHAR(MAX) NOT NULL
);

-- next, the user table, we do it in this order because of dependencies
CREATE TABLE USERS(
	user_id INT PRIMARY KEY,
	username VARCHAR(MAX) NOT NULL,
	password_hash VARCHAR(MAX) NOT NULL,
	email VARCHAR(MAX) NOT NULL,
	role_id INT REFERENCES USER_ROLE(role_id),
	created_at DATETIME,
	created_by INT
);

--next, we can do the farmer table
CREATE TABLE FARMER(
	farmer_id INT PRIMARY KEY,
	user_id INT REFERENCES USERS(user_id),
	farm_name VARCHAR(MAX) NOT NULL,
	farm_type VARCHAR(MAX) NOT NULL,
	havesting_date DATETIME NOT NULL,
	crop_type VARCHAR(MAX),
	livestock_type VARCHAR(MAX),
	number_of_employees INT NOT NULL,
);

--product table
CREATE TABLE PRODUCT(
	product_id INT PRIMARY KEY,
	product_name VARCHAR(MAX) NOT NULL,
	product_type VARCHAR(MAX) NOT NULL,
	product_description VARCHAR(MAX) NOT NULL,
	quantity INT,
	price DECIMAL(10,2),
	created_at DATETIME NOT NULL,
	updated_at DATETIME,
	farmer_id INT REFERENCES Farmer(farmer_id)
);

--ProductImage table
CREATE TABLE PRODUCT_IMAGE (
    image_id INT PRIMARY KEY,
    product_id INT FOREIGN KEY REFERENCES Product(product_id),
    image_data VARBINARY(MAX) NOT NULL,  
    content_type NVARCHAR(100) NOT NULL,  
    file_name NVARCHAR(255) NOT NULL,     
    created_at DATETIME2 NOT NULL
);
-- once tables are created, follow the readme and run the app to populate the database, ensuring that the
-- server name is changed to yours

-- to wipe, execute in this order
--DELETE FROM PRODUCT_IMAGE;
--DELETE FROM PRODUCT;
--DELETE FROM FARMER;
--DELETE FROM USERS;
--DELETE FROM USER_ROLE;

-- to check data, use this
--SELECT * FROM PRODUCT_IMAGE;
--SELECT * FROM PRODUCT;
--SELECT * FROM FARMER;
--SELECT * FROM USERS;
--SELECT * FROM USER_ROLE;
