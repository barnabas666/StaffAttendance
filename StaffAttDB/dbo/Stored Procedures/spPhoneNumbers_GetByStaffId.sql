CREATE PROCEDURE [dbo].[spPhoneNumbers_GetByStaffId]
	@staffId int	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [p].[Id], [p].[PhoneNumber]
	FROM dbo.PhoneNumbers p
	INNER JOIN dbo.StaffPhoneNumbers sp on p.Id = sp.PhoneNumberId
	WHERE sp.StaffId = @staffId
END
