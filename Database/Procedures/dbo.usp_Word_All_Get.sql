IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Word_All_Get]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Word_All_Get] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Word_All_Get]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-09-16
-- =======================================================
-- Description:	Gets all unknown words for the customer.
-- =======================================================
ALTER PROCEDURE[dbo].[usp_Word_All_Get]
	@CustomerId int
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT DISTINCT 
		w.WordId,
		English,
		Translation,
		IsKnown,
		Frequency
	FROM dbo.Word w		
	WHERE IsKnown = 0
		AND CustomerId = @CustomerId
END
GO
