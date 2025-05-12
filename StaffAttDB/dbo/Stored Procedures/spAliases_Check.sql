CREATE PROCEDURE [dbo].[spAliases_Check]	
	@alias nvarchar(10) 	
AS
BEGIN
	SET NOCOUNT ON;
SELECT
    CASE
        WHEN EXISTS( SELECT 1 FROM dbo.Aliases WHERE Alias = @alias )
            THEN 1 
        ELSE 0 
    END
END
