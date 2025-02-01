CREATE PROCEDURE [dbo].[spStaffs_GetAllBasic]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [s].[Id], [s].[FirstName], [s].[LastName], [s].[EmailAddress], 
	[s].[IsApproved], [d].[Description], [al].[Alias]
	FROM dbo.Staffs s
	INNER JOIN dbo.Departments d ON s.DepartmentId = d.Id
	INNER JOIN dbo.Aliases al ON s.AliasId = al.Id;
END
