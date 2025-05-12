CREATE PROCEDURE [dbo].[spPhoneNumbers_GetPhoneNumber]
	@phoneNumber nvarchar(50)	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT Id, PhoneNumber 
	FROM dbo.PhoneNumbers 
	WHERE PhoneNumber = @phoneNumber;
END
