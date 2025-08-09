SELECT 
    CASE 
        WHEN EXISTS (
            SELECT 1 FROM "Aliases" WHERE "Alias" = @alias
        ) THEN 1
        ELSE 0
    END;
