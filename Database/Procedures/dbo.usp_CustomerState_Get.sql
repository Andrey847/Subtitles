IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_CustomerState_Get]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_CustomerState_Get] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_CustomerState_Get]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-10-29
-- =======================================================
-- Description:	Returns state for the customer.
-- =======================================================
ALTER PROCEDURE[dbo].[usp_CustomerState_Get]
	@CustomerId int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT CustomerId,
		WorkPlaceLayout
	FROM dbo.CustomerState
	WHERE CustomerId = @CustomerId
END
GO