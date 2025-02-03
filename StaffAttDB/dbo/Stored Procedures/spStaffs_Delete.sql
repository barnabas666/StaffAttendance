CREATE PROCEDURE [dbo].[spStaffs_Delete]
    @staffId int
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Staffs
    WHERE id = @staffId;
END
