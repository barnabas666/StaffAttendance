SELECT * FROM dbo.Addresses;

  DECLARE @street nvarchar(100);
  DECLARE @city nvarchar(50);
  DECLARE @zip nvarchar(50);
  DECLARE @state nvarchar(50);
  
  SET @street = 'Test Street 123';
  SET @city = 'Test City';
  SET @zip = '12345';
  SET @state = 'Test State';

  INSERT INTO dbo.Addresses (Street, City, Zip, [State])
  VALUES (@street, @city, @zip, @state)

  DECLARE @insertedAddressId int;
  SET @insertedAddressId = SCOPE_IDENTITY();