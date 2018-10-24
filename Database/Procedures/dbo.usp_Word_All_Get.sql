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
	@CustomerId int,
	@MovieId int = NULL -- can be null. in this case return all words for this customer
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @UnknownRate int = 
	(
		SELECT Value 
		FROM dbo.CustomerSetting cs
			INNER JOIN dbo.Setting s
				ON cs.SettingId = s.SettingId
		WHERE CustomerId = @CustomerId	
			AND  s.Code = 'UnknownWordMax'
	)	
	
	SELECT w.WordId,
		w.Source,
		w.Translation,
		w.IsKnown,
		src.Frequency
	FROM
	(
		SELECT TOP (@UnknownRate) 
			w.WordId,	
			COUNT(pw.PhraseId) AS Frequency
		FROM dbo.Word w	
			INNER JOIN dbo.PhraseWord pw
				ON w.WordId = pw.WordId
			INNER JOIN dbo.Phrase p	
				ON pw.PhraseId = p.PhraseId
		WHERE IsKnown = 0
			AND CustomerId = @CustomerId	
			AND (@MovieId IS NULL OR p.MovieId = @MovieId)
		GROUP BY w.WordId
		ORDER BY Frequency DESC
	) src
		INNER JOIN dbo.Word w
			ON src.WordId = w.WordId
END
GO