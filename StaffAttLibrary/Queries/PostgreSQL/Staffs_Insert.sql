INSERT INTO "Staffs" ("DepartmentId", "AddressId", "AliasId", "FirstName", "LastName", "EmailAddress")
VALUES (@departmentId, @addressId, @aliasId, @firstName, @lastName, @emailAddress)
RETURNING "Id";
