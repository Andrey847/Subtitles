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
	@MovieId int
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM dbo.Movie
	WHERE MovieId = @MovieId;
END
GO