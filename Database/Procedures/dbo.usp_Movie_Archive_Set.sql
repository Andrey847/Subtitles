IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Movie_Archive_Set]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Movie_Archive_Set] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Movie_Archive_Set]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-10-27
-- =======================================================
-- Description:	Sets archive state for the movie.
-- =======================================================
ALTER PROCEDURE[dbo].[usp_Movie_Archive_Set]
	@CustomerId int,	
	@MovieId int,
	@IsArchived bit
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE dbo.Movie
	SET IsArchived = @IsArchived
	WHERE CustomerId = @CustomerId	-- customer id is additional protection.
		AND MovieId = @MovieId
END
GO