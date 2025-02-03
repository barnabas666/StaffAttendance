CREATE PROCEDURE [dbo].[spStaffs_GetBasicById]
	@id int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [s].[Id], [s].[FirstName], [s].[LastName], [s].[EmailAddress], 
	[s].[IsApproved], [s].[DepartmentId], [d].[Title], [al].[Alias]
	FROM dbo.Staffs s
	INNER JOIN dbo.Departments d ON s.DepartmentId = d.Id	
	INNER JOIN dbo.Aliases al ON s.AliasId = al.Id
	WHERE s.Id = @id;
END
