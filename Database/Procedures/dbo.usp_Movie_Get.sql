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
	@CustomerId int
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ShowArchived bit = 
	(
		SELECT cs.Value
		FROM CustomerSetting cs
			INNER JOIN dbo.Setting s
				ON cs.SettingId = s.SettingId
		WHERE cs.CustomerId = @CustomerId
			AND s.Code = N'ShowArchivedMovies'
	)

	DECLARE @LanguageId int = 
	(
		SELECT l.LanguageId 
		FROM dbo.CustomerSetting cs
			INNER JOIN dbo.Setting s
				ON cs.SettingId = s.SettingId
			INNER JOIN dbo.Language l
				ON cs.Value = l.Code
		WHERE CustomerId = @CustomerId	
			AND  s.Code = 'CurrentLanguageCode'
	)

	SELECT MovieId,
		Name,
		SubtitlesFileName,
		LanguageId,
		CustomerId,
		IsArchived
	FROM dbo.Movie
	WHERE CustomerId = @CustomerId
		AND (@ShowArchived = 1 OR IsArchived = 0) -- show all or not archived only.
		AND LanguageId = @LanguageId
	ORDER BY Name
END
GO