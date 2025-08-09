INSERT INTO "Addresses" ("Street", "City", "Zip", "State")
VALUES (@street, @city, @zip, @state)
RETURNING "Id";
