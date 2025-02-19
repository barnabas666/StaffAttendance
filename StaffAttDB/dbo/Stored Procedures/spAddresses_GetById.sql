CREATE PROCEDURE [dbo].[spAddresses_GetById]
	@id int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [ad].[Id], [ad].[Street], [ad].[City], [ad].[Zip], [ad].[State]
	FROM dbo.Addresses ad	
	WHERE ad.Id = @id;
END
