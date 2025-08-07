    UPDATE CheckIns
    SET CheckOutDate = CURRENT_TIMESTAMP
    WHERE Id = @checkInId;

    SELECT @checkInId;