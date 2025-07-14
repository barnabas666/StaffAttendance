INSERT INTO CheckIns (StaffId, CheckInDate)
VALUES (@staffId, CURRENT_TIMESTAMP);
SELECT last_insert_rowid();