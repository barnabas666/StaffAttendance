SELECT 
    s."FirstName", s."LastName", s."EmailAddress", d."Title",
    c."Id", c."StaffId", c."CheckInDate", c."CheckOutDate"
FROM "Staffs" s
INNER JOIN "CheckIns" c ON s."Id" = c."StaffId"
INNER JOIN "Departments" d ON s."DepartmentId" = d."Id"
WHERE s."Id" = @id
  AND @startDate <= c."CheckInDate"
  AND c."CheckInDate" < @endDate;
