CREATE PROCEDURE [dbo].[spCheckIns_GetByDateAndId]
	@id int,
	@startDate date,
  	@endDate date
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [s].[FirstName], [s].[LastName], [s].[EmailAddress], [d].Title,
	[c].[Id], [c].[StaffId], [c].[CheckInDate], [c].[CheckOutDate]	
	FROM dbo.Staffs s
	INNER JOIN dbo.CheckIns c ON s.Id = c.StaffId
	INNER JOIN dbo.Departments d ON s.DepartmentId = d.Id
	WHERE s.Id = @id AND @startDate <= c.CheckInDate AND c.CheckInDate < @endDate;
END
