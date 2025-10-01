namespace StaffAttShared.DTOs;

public class UpdateStaffRequest
{
    public AddressDto Address { get; set; }
    public string PIN { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public List<PhoneNumberDto> PhoneNumbers { get; set; }
}
