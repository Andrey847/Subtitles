IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Word_Sound_Get]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Word_Sound_Get] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Word_Sound_Get]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-09-16
-- =======================================================
-- Description:	Gets sound for the word.
-- =======================================================
ALTER PROCEDURE [dbo].[usp_Word_Sound_Get]
	@English nvarchar(100)
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT Wav
	FROM dbo.Word
	WHERE English = @English;
END
GO