IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Language_Get]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Language_Get] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Language_Get]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-10-08
-- =======================================================
-- Description:	Gets all languages in the system.
-- =======================================================
ALTER PROCEDURE[dbo].[usp_Language_Get]	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT LanguageId,
		Code,
		Name,
		BannerImage
	FROM dbo.[Language]
END
GO