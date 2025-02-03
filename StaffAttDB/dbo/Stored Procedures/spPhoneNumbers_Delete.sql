CREATE PROCEDURE [dbo].[spPhoneNumbers_Delete]
    @phoneNumberId int
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.PhoneNumbers
    WHERE id = @phoneNumberId;
END
