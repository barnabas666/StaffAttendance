CREATE PROCEDURE [dbo].[spStaffs_Insert]
	@departmentId int,
	@addressId int,
	@aliasId int,
	@firstName nvarchar(50),
	@lastName nvarchar(50),
	@emailAddress nvarchar(100)
AS
BEGIN
	INSERT INTO dbo.Staffs (DepartmentId, AddressId, AliasId, FirstName, LastName, EmailAddress)
	VALUES (@departmentId, @addressId, @aliasId, @firstName, @lastName, @emailAddress)
	SELECT SCOPE_IDENTITY();
END

