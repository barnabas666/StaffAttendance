CREATE PROCEDURE [dbo].[spAddresses_Insert]
	@street nvarchar(100),
	@city nvarchar(50),
	@zip nvarchar(50),
	@state nvarchar(50)
AS
BEGIN
	INSERT INTO dbo.Addresses (Street, City, Zip, [State])
	VALUES (@street, @city, @zip, @state)
	SELECT SCOPE_IDENTITY();
END