CREATE PROCEDURE [dbo].[spPhoneNumbers_Insert]
	@phoneNumber nvarchar(50)	
AS
BEGIN
	INSERT INTO dbo.PhoneNumbers (PhoneNumber)
	VALUES (@phoneNumber)
	SELECT SCOPE_IDENTITY();
END