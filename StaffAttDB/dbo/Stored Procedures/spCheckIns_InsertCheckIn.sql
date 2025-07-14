CREATE PROCEDURE [dbo].[spCheckIns_InsertCheckIn]
	@staffId int
AS
BEGIN	
	INSERT INTO dbo.CheckIns (StaffId, CheckInDate)
	VALUES (@StaffId, GETDATE())
	SELECT SCOPE_IDENTITY();
END
