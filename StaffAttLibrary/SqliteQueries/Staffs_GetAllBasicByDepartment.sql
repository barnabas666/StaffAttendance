SELECT 
    s.Id, s.DepartmentId, s.AddressId, s.AliasId, s.FirstName, 
    s.LastName, s.EmailAddress, s.IsApproved, 
    d.Title, d.Description, al.Alias
FROM Staffs s
INNER JOIN Departments d ON s.DepartmentId = d.Id
INNER JOIN Aliases al ON s.AliasId = al.Id
WHERE
    @departmentId = 0
    OR s.DepartmentId = @departmentId;