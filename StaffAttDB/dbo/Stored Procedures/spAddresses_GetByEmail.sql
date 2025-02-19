CREATE PROCEDURE [dbo].[spAddresses_GetByEmail]
	@emailAddress nvarchar(100)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [ad].[Id], [ad].[Street], [ad].[City], [ad].[Zip], [ad].[State]
	FROM dbo.Staffs s
	INNER JOIN dbo.Addresses ad ON s.AddressId = ad.Id
	WHERE EmailAddress = @emailAddress;
END
