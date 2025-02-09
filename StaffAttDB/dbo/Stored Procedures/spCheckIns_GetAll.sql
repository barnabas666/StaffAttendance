CREATE PROCEDURE [dbo].[spCheckIns_GetAll]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [s].[FirstName], [s].[LastName], [s].[EmailAddress], [d].Title,
	[c].[Id], [c].[StaffId], [c].[CheckInDate], [c].[CheckOutDate]	
	FROM dbo.Staffs s
	INNER JOIN dbo.CheckIns c ON s.Id = c.StaffId
	INNER JOIN dbo.Departments d ON s.DepartmentId = d.Id
END
