CREATE PROCEDURE [dbo].[spAliases_Insert]	
	@alias nvarchar(10),
	@pIN nvarchar(10) 	
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO dbo.Aliases (Alias, PIN)
	VALUES (@alias, @pIN)
	SELECT SCOPE_IDENTITY();
END
