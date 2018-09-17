IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Log_Add]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Log_Add] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Log_Add]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-09-17
-- =======================================================
-- Description:	Just adds log message.
-- =======================================================
ALTER PROCEDURE[dbo].[usp_Log_Add] 	
	@Message nvarchar(255),
	@Details nvarchar(4000) = NULL,
	@LogLevelId int,
	@Now datetime = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	INSERT INTO dbo.[Log] (LogLevelId, [Message], [Details], [DateStamp])
		VALUES (@LogLevelId, @Message, @Details, ISNULL(@Now, GETDATE()));
END
GO