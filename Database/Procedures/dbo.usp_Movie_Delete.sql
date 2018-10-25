IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Movie_Delete]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Movie_Delete] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Movie_Delete]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-10-24
-- =======================================================
-- Description:	Deletes movie.
-- =======================================================
ALTER PROCEDURE[dbo].[usp_Movie_Delete]
	@CustomerId int,	-- additional protection. Only owner can delete its movie.
	@MovieId int
AS
BEGIN
	SET NOCOUNT ON;

	-- Remove words that belongs to this movie only.
	DELETE w
	FROM uv_WordMovie m
		LEFT JOIN uv_WordMovie om-- other movie
			ON m.WordId = om.WordId
				AND om.CustomerId = @CustomerId	
				AND om.MovieId <> @MovieId
		INNER JOIN dbo.Word w
			ON m.WordId = w.WordId
	WHERE m.CustomerId = @CustomerId	
		AND m.MovieId = @MovieId
		AND om.WordId IS NULL	-- words that in this movie only
		AND w.Translation IS NULL	-- never delete word with translation
		AND w.IsKnown = 0	-- OR marked as known.

	DELETE FROM dbo.Movie
	WHERE MovieId = @MovieId
		AND CustomerId = @CustomerId;
END
GO
