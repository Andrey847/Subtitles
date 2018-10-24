IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Movie_Rename]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Movie_Rename] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Movie_Rename]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-10-24
-- =======================================================
-- Description:	Renames movie
-- =======================================================
ALTER PROCEDURE[dbo].[usp_Movie_Rename]
	@CustomerId int,	
	@MovieId int,
	@NewName nvarchar(250)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Error nvarchar(250) = '';

	SET XACT_ABORT ON
	BEGIN TRAN

		IF (EXISTS(
			SELECT * 
			FROM dbo.Movie
			WHERE CustomerId = @CustomerId
				AND MovieId <> @MovieId
				AND [Name] = @NewName
			))
		BEGIN
			SET @Error = 'Movie with this name already exists.'
		END
		ELSE
		BEGIN
			UPDATE dbo.Movie
			SET [Name] = @NewName,
				UpdatedWhen = GETDATE()
			WHERE CustomerId = @CustomerId
				AND MovieId = @MovieId
		END

	COMMIT TRAN

	SELECT @Error AS Error
END
GO