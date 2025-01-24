CREATE PROCEDURE [dbo].[spPhoneNumbers_GetByPhoneNumber]
	@phoneNumber nvarchar(50)	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT Id, PhoneNumber 
	FROM dbo.PhoneNumbers 
	WHERE PhoneNumber = @phoneNumber;
END