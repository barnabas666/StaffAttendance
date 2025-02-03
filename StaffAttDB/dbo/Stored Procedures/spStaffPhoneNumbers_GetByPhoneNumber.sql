CREATE PROCEDURE [dbo].[spStaffPhoneNumbers_GetByPhoneNumber]
	@phoneNumberId int	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [Id], [StaffId], [PhoneNumberId]
	FROM dbo.StaffPhoneNumbers sp	
	WHERE sp.PhoneNumberId = @phoneNumberId
END
