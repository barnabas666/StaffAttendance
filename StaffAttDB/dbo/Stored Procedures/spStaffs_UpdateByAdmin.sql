CREATE PROCEDURE [dbo].[spStaffs_UpdateByAdmin]
  	@id int,
    @departmentId int,
    @isApproved bit  	 
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Staffs
    SET DepartmentId = @departmentId, IsApproved = @isApproved
    WHERE id = @id;
END
