CREATE PROCEDURE [dbo].[spStaffPhoneNumbers_Insert]
	@staffId int,
	@phoneNumberId int	
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.StaffPhoneNumbers (StaffId, PhoneNumberId)
	VALUES (@staffId, @phoneNumberId);
END
