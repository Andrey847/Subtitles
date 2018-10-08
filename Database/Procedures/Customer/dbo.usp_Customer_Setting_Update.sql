IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Customer_Setting_Update]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Customer_Setting_Update] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Customer_Setting_Update]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-10-09
-- =======================================================
-- Description: Updates settings for Customer
-- =======================================================
ALTER PROCEDURE [dbo].[usp_Customer_Setting_Update]
	@CustomerId int,
	@CurrentLanguageCode nvarchar(10)
AS
BEGIN
	SET NOCOUNT ON;
	
	MERGE dbo.CustomerSetting AS Tgt
	USING
	(
		SELECT s.SettingId, @CustomerId AS CustomerId
		FROM dbo.Setting s
		WHERE s.Code = N'CurrentLanguageCode'		
	) AS Src
		ON Tgt.SettingId = Src.SettingId
			AND Tgt.CustomerId = Src.CustomerId
		WHEN MATCHED THEN 
			UPDATE SET [Value] = @CurrentLanguageCode,
						UpdatedWhen = GETDATE()
		WHEN NOT MATCHED THEN
			INSERT (CustomerId, SettingId, Value)
				VALUES (Src.CustomerId, Src.SettingId, @CurrentLanguageCode);
END
GO
