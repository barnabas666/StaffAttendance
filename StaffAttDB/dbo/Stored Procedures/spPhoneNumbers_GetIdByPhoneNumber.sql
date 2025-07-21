CREATE PROCEDURE [dbo].[spPhoneNumbers_GetIdByPhoneNumber]
	@phoneNumber nvarchar(50)	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT Id
	FROM dbo.PhoneNumbers 
	WHERE PhoneNumber = @phoneNumber;
END
