CREATE PROCEDURE [dbo].[spAliases_GetByAliasAndPIN]
	@alias nvarchar(10),
	@pIN nvarchar(10)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [Id], [Alias]
	FROM dbo.Aliases
	WHERE Alias = @alias AND PIN = @pIN;
END
