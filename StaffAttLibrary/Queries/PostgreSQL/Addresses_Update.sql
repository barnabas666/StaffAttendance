UPDATE "Addresses"
SET "Street" = @street,
    "City" = @city,
    "Zip" = @zip,
    "State" = @state
WHERE "Id" = @id;
