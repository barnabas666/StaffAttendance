CREATE PROCEDURE [dbo].[spAddresses_Update]
	@id int,
	@street nvarchar(100),
	@city nvarchar(50),
	@zip nvarchar(50),
	@state nvarchar(50)
AS
BEGIN
	UPDATE dbo.Addresses 
	SET Street = @street, City = @city, Zip = @zip, [State] = @state	
	WHERE Id = @id;
END
