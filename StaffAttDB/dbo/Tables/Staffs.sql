CREATE TABLE [dbo].[Staffs]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [DepartmentId] INT NOT NULL, 
    [AddressId] INT NOT NULL, 
    [AliasId] INT NOT NULL, 
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NOT NULL, 
    [EmailAddress] NVARCHAR(100) NOT NULL, 
    [IsApproved] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_Staffs_ToDepartments] FOREIGN KEY (DepartmentId) REFERENCES Departments(Id), 
    CONSTRAINT [FK_Staffs_ToAddresses] FOREIGN KEY (AddressId) REFERENCES Addresses(Id), 
    CONSTRAINT [FK_Staffs_ToAliases] FOREIGN KEY (AliasId) REFERENCES Aliases(Id)
)
