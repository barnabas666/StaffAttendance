CREATE PROCEDURE [dbo].[spAliases_Update]
	@id int,
	@pIN nvarchar(10)
AS
BEGIN
	UPDATE dbo.Aliases 
	SET PIN = @pIN	
	WHERE Id = @id;
END
