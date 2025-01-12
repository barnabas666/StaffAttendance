CREATE TABLE [dbo].[StaffPhoneNumbers]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [StaffId] INT NOT NULL, 
    [PhoneNumberId] INT NOT NULL, 
    CONSTRAINT [FK_StaffPhoneNumbers_ToStaff] FOREIGN KEY (StaffId) REFERENCES Staffs(Id), 
    CONSTRAINT [FK_StaffPhoneNumbers_ToPhoneNumbers] FOREIGN KEY (PhoneNumberId) REFERENCES PhoneNumbers(Id)
)
