SELECT 
    s."Id", s."FirstName", s."LastName", s."EmailAddress", 
    s."IsApproved", s."DepartmentId", d."Title", al."Alias"
FROM "Staffs" s
INNER JOIN "Departments" d ON s."DepartmentId" = d."Id"	
INNER JOIN "Aliases" al ON s."AliasId" = al."Id"
WHERE s."AliasId" = @aliasId;
