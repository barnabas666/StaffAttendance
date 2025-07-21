CREATE PROCEDURE [dbo].[spPhoneNumbers_Check]
	@phoneNumber nvarchar(50)	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
    CASE
        WHEN EXISTS( SELECT 1 FROM dbo.PhoneNumbers WHERE PhoneNumber = @phoneNumber )
            THEN 1 
        ELSE 0 
    END
END