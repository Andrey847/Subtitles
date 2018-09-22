IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Customer_RestoreCode_Verify]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Customer_RestoreCode_Verify] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Customer_RestoreCode_Verify]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-09-22
-- =======================================================
-- Description:Checks if this verification code exists in the DB and returns email if it is.
-- =======================================================
ALTER PROCEDURE[dbo].usp_Customer_RestoreCode_Verify
	@RestorePasswordCode nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LoginEmail nvarchar(255);
	DECLARE @Border datetime = DATEADD(day, -1, getdate());

	SET XACT_ABORT ON;
	BEGIN TRAN
		SET @LoginEmail = 
		(
			SELECT Email
			FROM Customer 
			WHERE RestorePasswordCode = @RestorePasswordCode
				AND RestoreDateTime >= @Border
		);

		IF @LoginEmail IS NOT NULL
		BEGIN
			-- Reset in order to do not accept the same code twice.
			UPDATE Customer
			SET RestorePasswordCode = NULL,
				RestoreDateTime = NULL
			WHERE Email = @LoginEmail;
		END		

	COMMIT TRAN

	SELECT @LoginEmail AS LoginEmail;
END
GO
