CREATE PROCEDURE [dbo].[spCheckIns_Delete]
    @StaffId int
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.CheckIns
    WHERE StaffId = @StaffId;
END