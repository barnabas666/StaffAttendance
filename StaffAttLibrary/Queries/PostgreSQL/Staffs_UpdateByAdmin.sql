UPDATE "Staffs"
SET "DepartmentId" = @departmentId,
    "IsApproved" = @isApproved
WHERE "Id" = @id;
