CREATE PROCEDURE [dbo].[spStaffs_GetBasicByEmail]
	@emailAddress nvarchar(100)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [s].[Id], [s].[DepartmentId], [s].[AddressId], [s].[AliasId], [s].[FirstName], 
	[s].[LastName], [s].[EmailAddress], [s].[IsApproved], [d].[Title], [d].[Description], [al].[Alias]
	FROM dbo.Staffs s
	INNER JOIN dbo.Departments d ON s.DepartmentId = d.Id
	INNER JOIN dbo.Aliases al ON s.AliasId = al.Id
	WHERE EmailAddress = @emailAddress;
END
