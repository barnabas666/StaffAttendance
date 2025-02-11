CREATE PROCEDURE [dbo].[spCheckIns_GetByDateAndEmail]
	@emailAddress nvarchar(100),
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
	WHERE s.EmailAddress = @emailAddress AND @startDate <= c.CheckInDate AND c.CheckInDate < @endDate;
END
