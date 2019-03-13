IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Movie_Get]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Movie_Get] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Movie_Get]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-09-16
-- =======================================================
-- Description:	Gets all movies for the customer
-- =======================================================
ALTER PROCEDURE[dbo].[usp_Movie_Get]
	@CustomerId int,
	@ShowArchivedMovies bit,
	@LanguageCode nvarchar(10)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LanguageId int = 
	(
		SELECT l.LanguageId 
		FROM dbo.Language l
		WHERE l.Code = @LanguageCode
	);

	SELECT MovieId,
		Name,
		SubtitlesFileName,
		LanguageId,
		CustomerId,
		IsArchived
	FROM dbo.Movie
	WHERE CustomerId = @CustomerId
		AND (@ShowArchivedMovies = 1 OR IsArchived = 0) -- show all or not archived only.
		AND LanguageId = @LanguageId
	ORDER BY Name
END
GO