INSERT INTO "PhoneNumbers" ("PhoneNumber")
VALUES (@phoneNumber)
RETURNING "Id";
