IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Customer_Setting_Get]') AND type IN (N'P', N'PC'))
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[usp_Customer_Setting_Get] AS RAISERROR(''Not implemented yet.'', 16, 3);')
END

PRINT ' Altering procedure [dbo].[usp_Customer_Setting_Get]'
GO

-- =======================================================
-- Author:		Andreev
-- Create date: 2018-10-04
-- =======================================================
-- Description:	Gets settings for Customer
-- =======================================================
ALTER PROCEDURE [dbo].[usp_Customer_Setting_Get]
	@CustomerId int
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT CustomerId,
		UnknownRate
	FROM 
	(
		SELECT cs.CustomerId, s.Code, cs.Value
		FROM dbo.CustomerSetting cs
			INNER JOIN dbo.Setting s
				ON cs.SettingId = s.SettingId
		WHERE cs.CustomerId = @CustomerId
			AND s.Code IN ('UnknownRate')
	) AS src
		PIVOT
		(
			MAX(Value) FOR Code IN ([UnknownRate])
		) pvt
END
GO