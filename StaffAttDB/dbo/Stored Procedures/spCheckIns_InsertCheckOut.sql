CREATE PROCEDURE [dbo].[spCheckIns_InsertCheckOut]
	@checkInId int
AS
BEGIN	
	UPDATE dbo.CheckIns
	SET CheckOutDate = GETDATE()
	WHERE Id = @checkInId
	SELECT SCOPE_IDENTITY();
END
