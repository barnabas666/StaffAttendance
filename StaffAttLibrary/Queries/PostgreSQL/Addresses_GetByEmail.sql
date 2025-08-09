SELECT a."Id", a."Street", a."City", a."Zip", a."State"
FROM "Addresses" a
JOIN "Staffs" s ON a."Id" = s."AddressId"
WHERE s."EmailAddress" = @emailAddress;
