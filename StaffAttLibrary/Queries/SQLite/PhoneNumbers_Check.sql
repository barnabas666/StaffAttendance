SELECT 
    CASE 
        WHEN EXISTS (SELECT 1 FROM PhoneNumbers WHERE PhoneNumber = @phoneNumber)
        THEN 1
        ELSE 0
    END;