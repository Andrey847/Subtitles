IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Rollout_Language]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Rollout_Language] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Rollout_Language]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-10-08
-- =======================================================
-- Description: Creates/updates langauge
-- =======================================================
ALTER PROCEDURE[dbo].[usp_Rollout_Language] 	
	@Code nvarchar(50),
	@Name nvarchar(50),
	@BannerImage nvarchar(255) = NULL	-- file name of banner. can be null.
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @LanguageId int = (SELECT LanguageId FROM dbo.[Language] WHERE Code = @Code)

	IF (@LanguageId IS NULL)
	BEGIN
		INSERT INTO dbo.[Language] (Code, [Name], BannerImage)
			VALUES (@Code, @Name, @BannerImage);
	END
	ELSE
	BEGIN
		UPDATE dbo.[Language]
		SET Name = @Name,
			BannerImage = @BannerImage
		WHERE LanguageId = @LanguageId
	END
END
GO