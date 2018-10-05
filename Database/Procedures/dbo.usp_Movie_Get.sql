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

	SELECT MovieId,
		Name,
		SubtitlesFileName,
		LanguageId,
		CustomerId
	FROM dbo.Movie
	WHERE CustomerId = @CustomerId
END
GO