CREATE PROCEDURE [dbo].[spCheckIns_GetLastRecord]
	@staffId int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT TOP (1) [c].[Id], [c].[StaffId], [c].[CheckInDate], [c].[CheckOutDate]
	FROM dbo.CheckIns c 
	INNER JOIN dbo.Staffs s ON c.StaffId = s.Id
	WHERE s.Id = @staffId
	ORDER BY CheckInDate DESC
END
