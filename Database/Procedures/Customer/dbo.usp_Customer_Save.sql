IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Customer_Save]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Customer_Save] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Customer_Save]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-09-18
-- =======================================================
-- Description:	Saves customer.
-- =======================================================
ALTER PROCEDURE [dbo].[usp_Customer_Save]
	@CustomerId int,
	@IsConfirmed bit,
	@ConfirmationCode nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	UPDATE dbo.Customer
	SET IsConfirmed = @IsConfirmed,
		ConfirmationCode = @ConfirmationCode,
		UpdatedWhen = getdate()
	WHERE CustomerId = @CustomerId;
END
GO