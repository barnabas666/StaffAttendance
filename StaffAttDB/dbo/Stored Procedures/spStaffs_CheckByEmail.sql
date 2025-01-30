CREATE PROCEDURE [dbo].[spStaffs_CheckByEmail]
	@emailAddress nvarchar(100)
AS
BEGIN
	SET NOCOUNT ON;

SELECT
    CASE
        WHEN EXISTS( SELECT 1 FROM dbo.Staffs WHERE EmailAddress = @emailAddress )
            THEN 1 
        ELSE 0 
    END
END
