INSERT INTO Aliases (Alias, PIN)
VALUES (@alias, @pIN);
SELECT last_insert_rowid();