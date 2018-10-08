IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Word_Translation_Save]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Word_Translation_Save] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Word_Translation_Save]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-09-16
-- =======================================================
-- Description:	Updates translation for the word.
-- =======================================================
ALTER PROCEDURE [dbo].[usp_Word_Translation_Save]
	@Source nvarchar(100),
	@Translation nvarchar(100)
AS
BEGIN
	UPDATE Word
	SET Translation = @Translation,
		UpdatedWhen = GETDATE()
	WHERE Source = @Source;
END
GO