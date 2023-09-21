:setvar DB TSD

IF NOT EXISTS(SELECT Name FROM sys.databases WHERE Name = '$(DB)') BEGIN
	CREATE DATABASE $(DB);
END

GO

USE $(DB);

GO

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Banks') BEGIN
	CREATE TABLE Banks (
		bank_name VARCHAR(255) NOT NULL, 
		password VARCHAR(255) NOT NULL, 
		CONSTRAINT PK_BANKS PRIMARY KEY (bank_name)
	);
END
ELSE BEGIN
	IF COL_LENGTH('dbo.Banks', 'bank_name') IS NULL BEGIN
		ALTER TABLE Banks ADD bank_name VARCHAR(255) NOT NULL;
	END
	IF COL_LENGTH('dbo.Banks', 'password') IS NULL BEGIN
		ALTER TABLE Banks ADD password VARCHAR(255) DEFAULT 'defpass' NOT NULL;
	END
END

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Screens') BEGIN
	EXEC sp_rename 'dbo.Screens', 'TicketingScreens';
END

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'TicketingScreens') BEGIN
	CREATE TABLE TicketingScreens (
		bank_name VARCHAR(255) NOT NULL, 
		screen_id INT IDENTITY(1, 1) NOT NULL,
		is_active BIT NOT NULL, 
		screen_title VARCHAR(255) NOT NULL, 
		CONSTRAINT PK_TICKETING_SCREENS PRIMARY KEY (bank_name, screen_id), 
		CONSTRAINT FK_TICKETING_SCREENS_BANKS FOREIGN KEY (bank_name) REFERENCES Banks(bank_name) ON DELETE CASCADE ON UPDATE CASCADE
	);
END
ELSE BEGIN
	IF COL_LENGTH('dbo.TicketingScreens', 'bank_name') IS NULL BEGIN
		ALTER TABLE TicketingScreens ADD bank_name VARCHAR(255) NOT NULL;
	END
	IF COL_LENGTH('dbo.TicketingScreens', 'screen_id') IS NULL BEGIN
		ALTER TABLE TicketingScreens ADD screen_id INT IDENTITY(1, 1) NOT NULL;
	END
	IF COL_LENGTH('dbo.TicketingScreens', 'is_active') IS NULL BEGIN
		ALTER TABLE TicketingScreens ADD is_active BIT NOT NULL;
	END
	IF COL_LENGTH('dbo.TicketingScreens', 'screen_title') IS NULL BEGIN
		ALTER TABLE TicketingScreens ADD screen_title VARCHAR(255) NOT NULL;
	END
END

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Buttons') BEGIN
	EXEC sp_rename 'dbo.Buttons', 'TicketingButtons';
END

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'TicketingButtons') BEGIN
	CREATE TABLE TicketingButtons (
		bank_name VARCHAR(255) NOT NULL, 
		screen_id INT NOT NULL, 
		button_id INT IDENTITY(1, 1) NOT NULL, 
		type INT NOT NULL, 
		name_en VARCHAR(255) NOT NULL, 
		name_ar NVARCHAR(255) NOT NULL, 
		service VARCHAR(255), 
		message_en VARCHAR(1000), 
		message_ar NVARCHAR(1000), 
		CONSTRAINT PK_TICKETING_BUTTONS PRIMARY KEY (bank_name, screen_id, button_id), 
		CONSTRAINT FK_TICKETING_BUTTONS_TICKETING_SCREENS FOREIGN KEY (bank_name, screen_id) REFERENCES TicketingScreens(bank_name, screen_id) ON DELETE CASCADE ON UPDATE CASCADE
	);
END
ELSE BEGIN
	IF COL_LENGTH('dbo.TicketingButtons', 'bank_name') IS NULL BEGIN
		ALTER TABLE TicketingButtons ADD bank_name VARCHAR(255) NOT NULL;
	END
	IF COL_LENGTH('dbo.TicketingButtons', 'screen_id') IS NULL BEGIN
		ALTER TABLE TicketingButtons ADD screen_id INT NOT NULL;
	END
	IF COL_LENGTH('dbo.TicketingButtons', 'button_id') IS NULL BEGIN
		ALTER TABLE TicketingButtons ADD button_id INT IDENTITY(1, 1) NOT NULL;
	END
	IF COL_LENGTH('dbo.TicketingButtons', 'type') IS NULL BEGIN
		ALTER TABLE TicketingButtons ADD type INT NOT NULL;
	END
	IF COL_LENGTH('dbo.TicketingButtons', 'name_en') IS NULL BEGIN
		ALTER TABLE TicketingButtons ADD name_en VARCHAR(255) NOT NULL;
	END
	IF COL_LENGTH('dbo.TicketingButtons', 'name_ar') IS NULL BEGIN
		ALTER TABLE TicketingButtons ADD name_ar NVARCHAR(255) NOT NULL;
	END
	IF COL_LENGTH('dbo.TicketingButtons', 'service') IS NULL BEGIN
		ALTER TABLE TicketingButtons ADD service VARCHAR(255);
	END
	IF COL_LENGTH('dbo.TicketingButtons', 'message_en') IS NULL BEGIN
		ALTER TABLE TicketingButtons ADD message_en VARCHAR(1000);
	END
	IF COL_LENGTH('dbo.TicketingButtons', 'message_ar') IS NULL BEGIN
		ALTER TABLE TicketingButtons ADD message_ar NVARCHAR(1000);
	END
END

DECLARE @drop_button_constraints_command VARCHAR(1000) = '';
SELECT @drop_button_constraints_command += 'ALTER TABLE TicketingButtons DROP CONSTRAINT ' + CONSTRAINT_NAME + '; ' FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
WHERE CONSTRAINT_NAME LIKE 'PK[_][_]TicketingButtons%' OR CONSTRAINT_NAME LIKE 'FK[_][_]TicketingButtons%';
EXECUTE(@drop_button_constraints_command);

DECLARE @drop_screen_constraints_command VARCHAR(1000) = '';
SELECT @drop_screen_constraints_command += 'ALTER TABLE TicketingScreens DROP CONSTRAINT ' + CONSTRAINT_NAME + '; ' FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
WHERE CONSTRAINT_NAME LIKE 'PK[_][_]TicketingScreens%' OR CONSTRAINT_NAME LIKE 'FK[_][_]TicketingScreens%';
EXECUTE(@drop_screen_constraints_command);

DECLARE @drop_bank_constraints_command VARCHAR(1000) = '';
SELECT @drop_bank_constraints_command += 'ALTER TABLE Banks DROP CONSTRAINT ' + CONSTRAINT_NAME + '; ' FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
WHERE CONSTRAINT_NAME LIKE 'PK[_][_]Banks%';
EXECUTE(@drop_bank_constraints_command);

GO

IF (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'PK_BANKS') = 0 BEGIN
	ALTER TABLE Banks ADD CONSTRAINT PK_BANKS PRIMARY KEY(bank_name);
END

IF (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'PK_TICKETING_SCREENS') = 0 BEGIN
	ALTER TABLE TicketingScreens ADD CONSTRAINT PK_TICKETING_SCREENS PRIMARY KEY(bank_name, screen_id);
END
IF (SELECT COUNT(*) FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_TICKETING_SCREENS_BANKS') = 0 BEGIN
	ALTER TABLE TicketingScreens ADD CONSTRAINT FK_TICKETING_SCREENS_BANKS FOREIGN KEY (bank_name) REFERENCES Banks(bank_name) ON DELETE CASCADE ON UPDATE CASCADE;
END

IF (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'PK_TICKETING_BUTTONS') = 0 BEGIN
	ALTER TABLE TicketingButtons ADD CONSTRAINT PK_TICKETING_BUTTONS PRIMARY KEY (bank_name, screen_id, button_id);
END
IF (SELECT COUNT(*) FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_TICKETING_BUTTONS_TICKETING_SCREENS') = 0 BEGIN
	ALTER TABLE TicketingButtons ADD CONSTRAINT FK_TICKETING_BUTTONS_TICKETING_SCREENS FOREIGN KEY (bank_name, screen_id) REFERENCES TicketingScreens(bank_name, screen_id) ON DELETE CASCADE ON UPDATE CASCADE;
END

DROP PROCEDURE IF EXISTS AddButtons;
DROP TYPE IF EXISTS add_buttons_parameter;
DROP TYPE IF EXISTS add_buttons_return;

CREATE TYPE add_buttons_parameter AS TABLE(
	bank_name VARCHAR(255) NOT NULL,
	screen_id INT NOT NULL,
	tmp_id INT NOT NULL,
	type INT NOT NULL,
	name_en VARCHAR(255) NOT NULL,
	name_ar NVARCHAR(255) NOT NULL,
	service VARCHAR(255),
	message_en VARCHAR(1000),
	message_ar NVARCHAR(1000)
);

CREATE TYPE add_buttons_return AS TABLE(
	bank_name VARCHAR(255) NOT NULL,
	screen_id INT NOT NULL,
	tmp_id INT NOT NULL,
	success BIT NOT NULL,
	error_number INT,
	error_message NVARCHAR(4000)
);

GO

CREATE PROCEDURE AddButtons (@buttons add_buttons_parameter READONLY) AS BEGIN
	DECLARE @return add_buttons_return;

	DECLARE @bank_name VARCHAR(255);
	DECLARE @screen_id INT;
	DECLARE @tmp_id INT;
	DECLARE @type INT;
	DECLARE @name_en VARCHAR(255);
	DECLARE @name_ar NVARCHAR(255);
	DECLARE @service VARCHAR(255);
	DECLARE @message_en VARCHAR(1000);
	DECLARE @message_ar NVARCHAR(1000);

	DECLARE parameter_cursor CURSOR FOR SELECT * FROM @buttons;
	OPEN parameter_cursor;

	FETCH NEXT FROM parameter_cursor INTO @bank_name, @screen_id, @tmp_id, @type, @name_en, @name_ar, @service, @message_en, @message_ar;

	WHILE @@FETCH_STATUS = 0 BEGIN
		BEGIN TRY
			INSERT INTO TicketingButtons (bank_name, screen_id, type, name_en, name_ar, service, message_en, message_ar) VALUES (@bank_name, @screen_id, @type, @name_en, @name_ar, @service, @message_en, @message_ar);
			INSERT INTO @return (bank_name, screen_id, tmp_id, success) VALUES (@bank_name, @screen_id, @tmp_id, 1);
		END TRY
		BEGIN CATCH
			INSERT INTO @return (bank_name, screen_id, tmp_id, success, error_number, error_message) VALUES (@bank_name, @screen_id, @tmp_id, 0, ERROR_NUMBER(), ERROR_MESSAGE());
		END CATCH

		FETCH NEXT FROM parameter_cursor INTO @bank_name, @screen_id, @tmp_id, @type, @name_en, @name_ar, @service, @message_en, @message_ar;
	END

	SELECT * FROM @return;
END

GO

DROP PROCEDURE IF EXISTS UpdateButtons;
DROP TYPE IF EXISTS update_buttons_parameter;
DROP TYPE IF EXISTS update_buttons_return;

GO

CREATE TYPE update_buttons_parameter AS TABLE(
	bank_name VARCHAR(255) NOT NULL,
	screen_id INT NOT NULL,
	button_id INT NOT NULL,
	type INT NOT NULL,
	name_en VARCHAR(255) NOT NULL,
	name_ar NVARCHAR(255) NOT NULL,
	service VARCHAR(255),
	message_en VARCHAR(1000),
	message_ar NVARCHAR(1000)
);

CREATE TYPE update_buttons_return AS TABLE(
	bank_name VARCHAR(255) NOT NULL,
	screen_id INT NOT NULL,
	button_id INT NOT NULL,
	success BIT NOT NULL,
	error_number INT,
	error_message NVARCHAR(4000)
);

GO

CREATE PROCEDURE UpdateButtons (@buttons update_buttons_parameter READONLY) AS BEGIN
	DECLARE @return update_buttons_return;

	DECLARE @bank_name VARCHAR(255);
	DECLARE @screen_id INT;
	DECLARE @button_id INT;
	DECLARE @type INT;
	DECLARE @name_en VARCHAR(255);
	DECLARE @name_ar NVARCHAR(255);
	DECLARE @service VARCHAR(255);
	DECLARE @message_en VARCHAR(1000);
	DECLARE @message_ar NVARCHAR(1000);

	DECLARE parameter_cursor CURSOR FOR SELECT * FROM @buttons;
	OPEN parameter_cursor;

	FETCH NEXT FROM parameter_cursor INTO @bank_name, @screen_id, @button_id, @type, @name_en, @name_ar, @service, @message_en, @message_ar;

	WHILE @@FETCH_STATUS = 0 BEGIN
		BEGIN TRY
			UPDATE TicketingButtons SET type = @type, name_en = @name_en, name_ar = @name_ar, service = @service, message_en = @message_en, message_ar = @message_ar WHERE bank_name = @bank_name AND screen_id = @screen_id AND button_id = @button_id;
			INSERT INTO @return (bank_name, screen_id, button_id, success) VALUES (@bank_name, @screen_id, @button_id, 1);
		END TRY
		BEGIN CATCH
			INSERT INTO @return (bank_name, screen_id, button_id, success, error_number, error_message) VALUES (@bank_name, @screen_id, @button_id, 0, ERROR_NUMBER(), ERROR_MESSAGE());
		END CATCH

		FETCH NEXT FROM parameter_cursor INTO @bank_name, @screen_id, @button_id, @type, @name_en, @name_ar, @service, @message_en, @message_ar;
	END

	SELECT * FROM @return;
END
