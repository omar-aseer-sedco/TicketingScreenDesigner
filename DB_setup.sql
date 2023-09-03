IF NOT EXISTS(SELECT Name FROM sys.databases WHERE Name = 'TSD') BEGIN
	CREATE DATABASE TSD;
END

GO

USE TSD;

GO

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Banks') BEGIN
	CREATE TABLE Banks (bank_name VARCHAR(255) NOT NULL, PRIMARY KEY (bank_name));
END

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Screens') BEGIN
	CREATE TABLE Screens (
		bank_name VARCHAR(255) NOT NULL, 
		screen_id VARCHAR(255) NOT NULL, 
		is_active BIT NOT NULL, 
		screen_title VARCHAR(255), 
		FOREIGN KEY (bank_name) REFERENCES Banks(bank_name) ON DELETE CASCADE ON UPDATE CASCADE, 
		PRIMARY KEY (bank_name, screen_id)
	);
END

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Buttons') BEGIN
	CREATE TABLE Buttons (
		bank_name VARCHAR(255) NOT NULL, 
		screen_id VARCHAR(255) NOT NULL, 
		button_id VARCHAR(255) NOT NULL, 
		type VARCHAR(255) NOT NULL, 
		name_en VARCHAR(255) NOT NULL, 
		name_ar NVARCHAR(255) NOT NULL, 
		service VARCHAR(255), 
		message_en VARCHAR(1000), 
		message_ar NVARCHAR(1000), 
		FOREIGN KEY (bank_name, screen_id) REFERENCES Screens(bank_name, screen_id) ON DELETE CASCADE ON UPDATE CASCADE, 
		PRIMARY KEY (bank_name, screen_id, button_id)
	);
END