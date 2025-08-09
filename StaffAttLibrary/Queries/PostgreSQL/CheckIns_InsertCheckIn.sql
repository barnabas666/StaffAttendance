INSERT INTO "CheckIns" ("StaffId", "CheckInDate")
VALUES (@staffId, CURRENT_TIMESTAMP)
RETURNING "Id";
