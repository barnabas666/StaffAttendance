    SELECT 
        c.Id, c.StaffId, c.CheckInDate, c.CheckOutDate
    FROM CheckIns c
    INNER JOIN Staffs s ON c.StaffId = s.Id
    WHERE s.Id = @staffId
    ORDER BY c.CheckInDate DESC
    LIMIT 1;