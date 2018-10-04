IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Rollout_Setting]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Rollout_Setting] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Rollout_Setting]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-10-04
-- =======================================================
-- Description: Creates/Updates setting
-- =======================================================
ALTER PROCEDURE[dbo].[usp_Rollout_Setting] 	
	@Code nvarchar(50),
	@Name nvarchar(50),
	@DefaultValue nvarchar(1000),
	@Description nvarchar(1000)	
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @SettingId int = 
	(
		SELECT TOP 1 SettingId
		FROM dbo.Setting
		WHERE Code = @Code
	);

	IF (@SettingId IS NULL)
	BEGIN
		INSERT INTO dbo.Setting (Code, [Name], [Description], DefaultValue)
			VALUES (@Code, @Name, @Description, @DefaultValue)

		SET @SettingId = SCOPE_IDENTITY();

		-- And create this setting for all users
		INSERT INTO dbo.CustomerSetting (CustomerId, SettingId, Value)
		SELECT CustomerId, @SettingId, @DefaultValue
		FROM dbo.Customer		
	END
	ELSE
	BEGIN
		UPDATE dbo.Setting
		SET [Name] = @Name,
			[Description] = @Description,
			DefaultValue = @DefaultValue
		WHERE SettingId = @SettingId
	END
END
GO