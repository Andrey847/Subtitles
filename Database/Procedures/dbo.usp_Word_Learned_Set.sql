IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Word_Learned_Set]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Word_Learned_Set] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Word_Learned_Set]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-09-16
-- =======================================================
-- Description:	Marks word as learned or unlearned (in case of undo action).
-- =======================================================
ALTER PROCEDURE [dbo].[usp_Word_Learned_Set]
	@CustomerId int,
	@Source nvarchar(100), -- word itself
	@IsKnown bit
AS
BEGIN
	UPDATE Word
	SET IsKnown = @IsKnown,
		UpdatedWhen = getdate()
	WHERE Source = @Source
		AND CustomerId = @CustomerId;
END
GO

