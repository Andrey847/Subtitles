IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Phrase_Merge]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Phrase_Merge] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Phrase_Merge]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-09-16
-- =======================================================
-- Description:	Adds (if it does not exist yet) and returns word info (is word known or not).
-- =======================================================
ALTER PROCEDURE [dbo].[usp_Phrase_Merge]
	@CustomerId int,
	@FileName nvarchar(260) = NULL,
	@Value nvarchar(400),	
	@TimeFrom time(7),
	@TimeTo time(7),
	@OrderNumber int,
	@Words xml,
	@LanguageId int = 1 -- eng
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @MovieId int = (SELECT MovieId FROM dbo.Movie WHERE SubtitlesFileName = @FileName)

	IF (@MovieId IS NULL)
	BEGIN
		INSERT INTO dbo.Movie (Name, SubtitlesFileName, LanguageId, CustomerId)
			VALUES (@FileName, @FileName, @LanguageId, @CustomerId)

		SET @MovieId = SCOPE_IDENTITY();
	END


	DECLARE @PhraseId int = 
	(
		SELECT TOP 1 PhraseId
		FROM dbo.Phrase
		WHERE MovieId = @MovieId
			AND OrderNumber = @OrderNumber
			AND TimeFrom = @TimeFrom
			AND TimeTo = @TimeTo
			AND Value = @Value
	)

	IF (@PhraseId IS NULL)
	BEGIN
		INSERT INTO dbo.Phrase(MovieId, TimeFrom, TimeTo, OrderNumber, Value)
			VALUES (@MovieId, @TimeFrom, @TimeTo, @OrderNumber, @Value)

			SET @PhraseId = SCOPE_IDENTITY();
	END

	-- And save phrases (only new)
	SELECT DISTINCT R.c.value('.[1]', 'nvarchar(100)') Word		
	INTO #Words
	FROM @Words.nodes('ArrayOfString/string') AS R(c);	

	INSERT INTO dbo.Word 
	(
		Source,
		Translation,
		IsKnown,
		Wav,
		CustomerId
	)
	SELECT Word,
		NULL,
		0,
		NULL,
		@CustomerId
	FROM #Words w
		LEFT JOIN dbo.Word ww
			ON w.Word = ww.Source 
				AND ww.CustomerId = @CustomerId
	WHERE ww.WordId IS NULL;

	DECLARE @NewWords int = @@ROWCOUNT;

	INSERT INTO PhraseWord (PhraseId, WordId)
	SELECT @PhraseId, ww.WordId
	FROM #Words w
		INNER JOIN dbo.Word ww
			ON w.Word = ww.Source
	WHERE ww.CustomerId = @CustomerId

	SELECT @NewWords AS NewWords

	DROP TABLE #Words;
END
GO