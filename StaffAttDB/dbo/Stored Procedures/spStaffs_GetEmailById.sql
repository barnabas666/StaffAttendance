CREATE PROCEDURE [dbo].[spStaffs_GetEmailById]
	@id int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [s].[EmailAddress]	
	FROM dbo.Staffs s
	WHERE s.Id = @id;
END
