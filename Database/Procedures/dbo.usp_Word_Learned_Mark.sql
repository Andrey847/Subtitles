IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Word_Merge]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Word_Merge] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Word_Merge]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-09-16
-- =======================================================
-- Description:	Marks word as learned.
-- =======================================================
ALTER PROCEDURE [dbo].[usp_Word_Learned_Mark]
	@English nvarchar(100)
AS
BEGIN
	UPDATE Word
	SET IsKnown = 1,
		UpdatedWhen = getdate()
	WHERE English = @English;
END
GO

