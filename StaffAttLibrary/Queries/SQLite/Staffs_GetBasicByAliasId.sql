SELECT s.Id, s.FirstName, s.LastName, s.EmailAddress, a.Alias, s.IsApproved, s.DepartmentId, d.Title
FROM Staffs s
JOIN Aliases a ON s.AliasId = a.Id
JOIN Departments d ON s.DepartmentId = d.Id
WHERE s.AliasId = @aliasId;