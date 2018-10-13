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
	@CustomerId int,
	@Source nvarchar(100),
	@Frequency int,
	@FileName nvarchar(260) = NULL,
	@Phrases xml,
	@LanguageId int = 1 -- eng
AS
BEGIN
	DECLARE @MovieId int = (SELECT MovieId FROM dbo.Movie WHERE SubtitlesFileName = @FileName)

	IF (@MovieId IS NULL)
	BEGIN
		INSERT INTO dbo.Movie (Name, SubtitlesFileName, LanguageId, CustomerId)
			VALUES (@FileName, @FileName, @LanguageId, @CustomerId)

		SET @MovieId = SCOPE_IDENTITY();
	END

	DECLARE @IsAdded bit = 0;

	IF NOT EXISTS(SELECT * FROM Word WHERE Source = @Source)
	BEGIN
		INSERT INTO dbo.Word (Source, Translation, IsKnown, Frequency, CustomerId)
			VALUES (@Source, NULL, 0, @Frequency, @CustomerId)

		SET @IsAdded = 1;
	END
	ELSE
	BEGIN
		UPDATE dbo.Word
		SET Frequency = Frequency + @Frequency
		WHERE Source = @Source
	END

	DECLARE @WordId int = (SELECT WordId FROM [dbo].[Word] WHERE Source = @Source);

	-- And save phrases (only new)
	SELECT DISTINCT	R.c.value('Value[1]', 'nvarchar(500)') Phrase,
		R.c.value('TimeFromSql[1]', 'time(7)') AS TimeFrom,
			R.c.value('TimeToSql[1]', 'time(7)') AS TimeTo,
			R.c.value('OrderNumber[1]', 'int') AS OrderNumber
	INTO #Phrases
	FROM @Phrases.nodes('ArrayOfPhrase/Phrase') AS R(c);
	
	INSERT INTO Phrase (Value, MovieId, TimeFrom, TimeTo, OrderNumber)
	SELECT src.Phrase, @MovieId, src.TimeFrom, src.TimeTo, src.OrderNumber
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

	SELECT @IsAdded AS IsAdded

	DROP TABLE #Phrases;
END
GO