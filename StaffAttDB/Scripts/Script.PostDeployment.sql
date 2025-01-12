/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

if not exists (select 1 from dbo.Departments)
begin
    insert into dbo.Departments(Title, Description)
    values ('HR', 'Human resources department.'),
    ('Management', 'Management department.'),
    ('IT', 'IT department.'),
    ('Marketing', 'Marketing department.'),
    ('Accounting', 'Accounting department.');
end
