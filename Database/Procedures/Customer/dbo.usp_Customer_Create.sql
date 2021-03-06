﻿IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Customer_Create]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Customer_Create] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Customer_Create]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-09-17
-- =======================================================
-- Description:	Creates customer.
-- =======================================================
ALTER PROCEDURE [dbo].[usp_Customer_Create]
	@Email nvarchar(250),
	@PasswordHash nvarchar(255),
	@ConfirmationCode nvarchar(50),
	@IsConfirmed bit
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS
	( 
		SELECT * 
		FROM dbo.Customer 
		WHERE Email = @Email
	)
	BEGIN
		RAISERROR('Customer with administrative email %s already exists', 16, 1, @Email);
		RETURN
	END
	ELSE
	BEGIN

		DECLARE @UserRoleId int = (SELECT CustomerRoleId FROM CustomerRole WHERE Code = N'user');

		INSERT INTO dbo.Customer (Email, 
									Name,
									PasswordHash,
									IsBlocked,									
									CustomerRoleId,									
									IsConfirmed	,
									ConfirmationCode
									)
			VALUES (@Email,
				@Email,
				@PasswordHash, 
				CASE 
					WHEN @IsConfirmed = 1 THEN 0 	-- unblocked if confirmed
					ELSE 1							-- and blocked if not.
				END,
				@UserRoleId,
				@IsConfirmed,
				@ConfirmationCode);
	END
END
GO