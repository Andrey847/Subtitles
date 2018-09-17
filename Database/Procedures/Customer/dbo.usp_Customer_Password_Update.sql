IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Customer_Password_Update]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Customer_Password_Update] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Customer_Password_Update]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-09-17
-- =======================================================
-- Description:	Updates password hash for the customer.
-- =======================================================
ALTER PROCEDURE [dbo].[usp_Customer_Password_Update]
	@Email nvarchar(250),
	@PasswordHash nvarchar(255)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT EXISTS
	( 
		SELECT * 
		FROM dbo.Customer 
		WHERE Email = @Email
	)
	BEGIN
		RAISERROR('Customer with administrative email %s not found', 16, 1, @Email);
		RETURN
	END
	ELSE
	BEGIN
		UPDATE dbo.Customer
		SET PasswordHash = @PasswordHash
		WHERE Email = @Email;
	END
END
GO