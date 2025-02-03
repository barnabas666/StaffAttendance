CREATE PROCEDURE [dbo].[spStaffPhoneNumbers_Delete]
    @staffId int,
    @phoneNumberId int
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.StaffPhoneNumbers
    WHERE StaffId = @staffId AND PhoneNumberId = @phoneNumberId;
END
