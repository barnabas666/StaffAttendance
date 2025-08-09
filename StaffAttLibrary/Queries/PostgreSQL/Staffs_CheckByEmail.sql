SELECT 
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM "Staffs" 
            WHERE "EmailAddress" = @emailAddress
        )
        THEN 1 
        ELSE 0 
    END;
