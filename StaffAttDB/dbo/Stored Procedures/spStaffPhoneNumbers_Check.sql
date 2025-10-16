CREATE PROCEDURE [dbo].[spStaffPhoneNumbers_Check]
	@staffId int,
    @phoneNumberId int	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
    CASE
        WHEN EXISTS( SELECT 1 FROM dbo.StaffPhoneNumbers WHERE StaffId = @staffId AND PhoneNumberId = @phoneNumberId )
            THEN 1 
        ELSE 0 
    END
END
