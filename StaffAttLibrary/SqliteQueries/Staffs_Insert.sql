INSERT INTO Staffs (DepartmentId, AddressId, AliasId, FirstName, LastName, EmailAddress)
VALUES (@departmentId, @addressId, @aliasId, @firstName, @lastName, @emailAddress);
SELECT last_insert_rowid();