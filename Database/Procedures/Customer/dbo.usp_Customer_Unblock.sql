IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Customer_Unblock]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Customer_Unblock] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Customer_Unblock]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2019-03-14
-- =======================================================
-- Description:	Unblocks customer by confirmation code 
-- =======================================================
ALTER PROCEDURE [dbo].[usp_Customer_Unblock]
	@ConfirmationCode nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;

	SET XACT_ABORT ON;
	BEGIN TRAN

		DECLARE @Email nvarchar(250) = 
		(
			SELECT TOP 1 Email
			FROM dbo.Customer
			WHERE ConfirmationCode = @ConfirmationCode
		)

		IF (@Email IS NOT NULL)
		BEGIN	
			UPDATE dbo.Customer
			SET IsBlocked = 0,
				UpdatedWhen = GETDATE(),
				ConfirmationCode = NULL
			WHERE Email = @Email				
		END

	COMMIT TRAN

	SELECT @Email AS Email
END
GO