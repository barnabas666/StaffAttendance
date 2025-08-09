UPDATE "Staffs"
SET "FirstName" = @firstName,
    "LastName" = @lastName,
    "IsApproved" = @isApproved
WHERE "Id" = @id;
