CREATE PROCEDURE [dbo].[spStaffs_GetByAlias]
	@aliasId int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [s].[Id], [s].[DepartmentId], [s].[AddressId], [s].[AliasId], [s].[FirstName], 
	[s].[LastName], [s].[EmailAddress], [s].[IsApproved], [d].[Title], [d].[Description], 
	[ad].[Street], [ad].[City], [ad].[Zip], [ad].[State], [al].[Alias]
	FROM dbo.Staffs s  
	LEFT JOIN dbo.CheckIns c ON s.Id = c.StaffId
	INNER JOIN dbo.Departments d ON s.DepartmentId = d.Id
	INNER JOIN dbo.Addresses ad ON s.AddressId = ad.Id
	INNER JOIN dbo.Aliases al ON s.AliasId = al.Id
	WHERE s.AliasId = @aliasId;	
END
