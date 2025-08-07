INSERT INTO PhoneNumbers (PhoneNumber)
VALUES (@phoneNumber);
SELECT last_insert_rowid();