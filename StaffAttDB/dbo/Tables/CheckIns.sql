CREATE TABLE [dbo].[CheckIns]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [StaffId] INT NOT NULL, 
    [CheckInDate] DATETIME2 NOT NULL, 
    [CheckOutDate] DATETIME2 NULL, 
    CONSTRAINT [FK_CheckIns_ToStaffs] FOREIGN KEY (StaffId) REFERENCES Staffs(Id)
)
