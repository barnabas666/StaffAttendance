CREATE PROCEDURE [dbo].[spStaffs_Update]
	@id int,
	@firstName nvarchar(50),
	@lastName nvarchar(50),
	@isApproved bit
AS
BEGIN
	UPDATE dbo.Staffs 
	SET FirstName = @firstName, LastName = @lastName, IsApproved = @isApproved	
	WHERE Id = @id;
END