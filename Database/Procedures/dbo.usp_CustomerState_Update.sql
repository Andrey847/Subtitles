IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_CustomerState_Update]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_CustomerState_Update] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_CustomerState_Update]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-10-29
-- =======================================================
-- Description:	Updates state for the customer.
-- =======================================================
ALTER PROCEDURE[dbo].[usp_CustomerState_Update]
	@CustomerId int,
	@WorkPlaceLayout nvarchar(max) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SET XACT_ABORT ON;
	BEGIN TRAN

		IF EXISTS(SELECT * FROM dbo.CustomerState WHERE CustomerId = @CustomerId)
		BEGIN
			UPDATE dbo.CustomerState
			SET WorkPlaceLayout = ISNULL(NULLIF(@WorkPlaceLayout, ''), WorkPlaceLayout)
			WHERE CustomerId = @CustomerId;
		END
		ELSE
		BEGIN
			INSERT INTO dbo.CustomerState (CustomerId, WorkPlaceLayout)
				VALUES (@CustomerId, @WorkPlaceLayout);
		END

	COMMIT TRAN
END
GO