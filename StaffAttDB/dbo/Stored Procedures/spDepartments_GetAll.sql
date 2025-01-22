CREATE PROCEDURE [dbo].[spDepartments_GetAll]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [Id], [Title], [Description]
	FROM dbo.Departments
END
