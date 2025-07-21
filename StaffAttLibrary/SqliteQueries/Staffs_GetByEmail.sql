SELECT s.Id, s.DepartmentId, s.AddressId, s.AliasId, s.FirstName, s.LastName, 
	   s.EmailAddress, s.IsApproved, d.Title, d.Description, a.Alias
FROM Staffs s
JOIN Aliases a ON s.AliasId = a.Id
JOIN Departments d ON s.DepartmentId = d.Id
WHERE s.EmailAddress = @emailAddress;