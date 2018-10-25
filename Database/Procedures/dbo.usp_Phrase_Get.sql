IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Phrase_Get]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Phrase_Get] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Phrase_Get]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-10-05
-- =======================================================
-- Description:	Returns phrases, related to specific word.
-- =======================================================
ALTER PROCEDURE[dbo].[usp_Phrase_Get]
	@WordId int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT p.Value
	FROM Phrase p
		INNER JOIN PhraseWord pw	
			ON p.PhraseId = pw.PhraseId
	WHERE pw.WordId = @WordId
END
GO