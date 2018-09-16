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
-- Description:	Adds (if it does not exist yet) and returns word info (is word known or not).
-- =======================================================
ALTER PROCEDURE [dbo].[usp_Word_Merge]
	@English nvarchar(100),
	@Frequency int,
	@FileName nvarchar(260) = NULL,
	@Phrases xml,
	@LanguageId int = 1 -- eng
AS
BEGIN
	DECLARE @MovieId int = (SELECT MovieId FROM dbo.Movie WHERE SubtitlesFileName = @FileName)

	IF (@MovieId IS NULL)
	BEGIN
		INSERT INTO dbo.Movie (Name, SubtitlesFileName, LanguageId)
			VALUES (@FileName, @FileName, @LanguageId)

		SET @MovieId = SCOPE_IDENTITY();
	END

	IF NOT EXISTS(SELECT * FROM Word WHERE English = @English)
	BEGIN
		INSERT INTO dbo.Word (English, Translation, IsKnown, Frequency)
			VALUES (@English, NULL, 0, @Frequency)
	END
	ELSE
	BEGIN
		UPDATE dbo.Word
		SET Frequency = Frequency + @Frequency
		WHERE English = @English
	END

	DECLARE @WordId int = (SELECT WordId FROM [dbo].[Word] WHERE English = @English);

	IF NOT EXISTS (SELECT * FROM [dbo].[MovieWord] WHERE MovieId = @MovieId AND WordId = @WordId)
	BEGIN
		INSERT INTO MovieWord (MovieId, WordId)
			VALUES (@MovieId, @WordId)
	END

	-- And save phrases (only new)
	SELECT DISTINCT	R.c.value('Value[1]', 'nvarchar(500)') Phrase		
	INTO #Phrases
	FROM @Phrases.nodes('ArrayOfPhrase/Phrase') AS R(c);

	INSERT INTO Phrase (Value, MovieId)
	SELECT src.Phrase, @MovieId
	FROM #Phrases src
		LEFT JOIN Phrase tgt
			ON src.Phrase = tgt.Value
	WHERE tgt.PhraseId IS NULL
		
	INSERT INTO PhraseWord (PhraseId, WordId)
	SELECT p2.PhraseId, @WordId
	FROM #Phrases p1
		INNER JOIN Phrase p2
			ON p1.Phrase = p2.Value
		LEFT JOIN PhraseWord pw
			ON pw.WordId = @WordId
				AND pw.PhraseId = p2.PhraseId
	WHERE pw.PhraseWordId IS NULL

	SELECT WordId,
		English,
		Translation,
		IsKnown
	FROM dbo.Word
	WHERE English = @English;

	DROP TABLE #Phrases;
END
GO

