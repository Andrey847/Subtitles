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

	-- use different queries for the performance reasons
	IF (@MovieId IS NULL)
	BEGIN	
		SELECT TOP (@UnknownRate) 
			w.WordId,
			Source,
			Translation,
			IsKnown,
			Frequency
		FROM dbo.Word w				
		WHERE IsKnown = 0
			AND CustomerId = @CustomerId
		ORDER BY Frequency DESC
	END
	ELSE
	BEGIN
		
		SELECT TOP (@UnknownRate) 
			src.WordId,
			src.Source,
			src.Translation,
			src.IsKnown,
			src.Frequency
		FROM 
		(
			-- distinct as each word might be in different phrases
			SELECT DISTINCT w.WordId,
				w.Source,
				w.Translation,
				w.IsKnown,
				w.Frequency
			FROM dbo.Word w			
				INNER JOIN dbo.PhraseWord pw
					ON w.WordId = pw.WordId
				INNER JOIN dbo.Phrase p
					ON pw.PhraseId = p.PhraseId
			WHERE IsKnown = 0
				AND CustomerId = @CustomerId
				AND p.MovieId = @MovieId
		) src
		ORDER BY Frequency DESC
	END
END
GO