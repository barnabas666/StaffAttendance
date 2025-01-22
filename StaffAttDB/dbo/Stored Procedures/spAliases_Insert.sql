CREATE PROCEDURE [dbo].[spAliases_CheckIfAvailable]
	@alias nvarchar(10),
	@pIN nvarchar(10)
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM dbo.Aliases WHERE Alias = @alias)
	BEGIN
		INSERT INTO dbo.Aliases (Alias, PIN)
		VALUES (@alias, @pIN);
		SELECT SCOPE_IDENTITY();
	END
END