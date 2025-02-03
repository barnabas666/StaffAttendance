CREATE PROCEDURE [dbo].[spAddresses_Delete]
    @id int
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Addresses
    WHERE id = @id;
END
