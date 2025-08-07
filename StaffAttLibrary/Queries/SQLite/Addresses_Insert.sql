INSERT INTO Addresses (Street, City, Zip, State)
VALUES (@street, @city, @zip, @state);
SELECT last_insert_rowid();