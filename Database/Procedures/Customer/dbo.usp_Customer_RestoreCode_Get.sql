IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Customer_RestoreCode_Get]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Customer_RestoreCode_Get] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Customer_RestoreCode_Get]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-09-22
-- =======================================================
-- Description:	Creates and returns restore password code for the Customer.
-- =======================================================
ALTER PROCEDURE[dbo].usp_Customer_RestoreCode_Get
	@Email nvarchar(255)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ResultCode nvarchar(50) = ''

	IF EXISTS(SELECT * FROM Customer WHERE [Email] = @Email)
	BEGIN
		SET XACT_ABORT ON;

		BEGIN TRAN

			SET @ResultCode = NEWID();

			UPDATE Customer
			SET RestorePasswordCode = @ResultCode,
				RestoreDateTime = GETDATE()
			WHERE [Email] = @Email;

		COMMIT TRAN
	END

	SELECT @ResultCode AS RestorePasswordCode
END
GO
