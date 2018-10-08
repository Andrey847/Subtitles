IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Word_Sound_Add]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Word_Sound_Add] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Word_Sound_Add]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-09-16
-- =======================================================
-- Description:	Adds sound for word.
-- =======================================================
ALTER PROCEDURE [dbo].[usp_Word_Sound_Add]
	@Source nvarchar(100),
	@Wav varbinary(max)
AS
BEGIN
	SET NOCOUNT ON;
	
	UPDATE dbo.Word
	SET Wav = @Wav
	WHERE Source = @Source;
END
GO