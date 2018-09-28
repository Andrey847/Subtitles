IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Customer_Get]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Customer_Get] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Customer_Get]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-09-17
-- =======================================================
-- Description:	Returns customer info by email or exact customerId.
-- =======================================================
ALTER PROCEDURE [dbo].[usp_Customer_Get]
	@Email nvarchar(250) = NULL,
	@CustomerId int = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	IF @Email IS NULL AND @CustomerId IS NULL
	BEGIN
		RAISERROR('Please provide email or customerId', 16, 1);
		RETURN
	END
	ELSE IF (@Email IS NOT NULL AND @CustomerId IS NOT NULL)
	BEGIN
		RAISERROR('Please provide email or customerId only. Not both.', 16, 1);
		RETURN
	END
	ELSE
	BEGIN

		SELECT CustomerId,
			Email,
			[Name],
			IsConfirmed,
			IsBlocked,
			PasswordHash,
			RestorePasswordCode,
			ConfirmationCode,
			CustomerRoleId
		FROM dbo.Customer c
		WHERE Email = @Email OR CustomerId = @CustomerId
	END
END
GO