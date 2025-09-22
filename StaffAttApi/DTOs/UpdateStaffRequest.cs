using StaffAttLibrary.Models;

namespace StaffAttApi.DTOs;

/// <summary>
/// Represents a request to update the details of a staff member.
/// </summary>
/// <remarks>This class is used to encapsulate the information required to update a staff member's details,
/// including their address, personal information, and contact details.</remarks>
public class UpdateStaffRequest
{
    public AddressModel Address { get; set; }
    public string PIN { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public List<PhoneNumberModel> PhoneNumbers { get; set; }
}
