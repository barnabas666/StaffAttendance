CREATE PROCEDURE [dbo].[spAliases_Delete]
    @id int
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Aliases
    WHERE id = @id;
END