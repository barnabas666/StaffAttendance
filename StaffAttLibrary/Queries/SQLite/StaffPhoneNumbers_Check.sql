SELECT 
    CASE 
        WHEN EXISTS (SELECT 1 FROM StaffPhoneNumbers WHERE StaffId = @staffId AND PhoneNumberId = @phoneNumberId)
        THEN 1
        ELSE 0
    END;