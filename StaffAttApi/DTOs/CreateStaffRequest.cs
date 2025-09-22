using StaffAttLibrary.Models;

namespace StaffAttApi.DTOs;

/// <summary>
/// Represents a request to create a new staff member, including personal, contact, and department details.
/// </summary>
/// <remarks>This class is used to encapsulate the data required to create a staff member in the system. It
/// includes information such as the staff member's name, contact details, and the department they belong to.</remarks>
public class CreateStaffRequest
{
    public int DepartmentId { get; set; }
    public AddressModel Address { get; set; }
    public string PIN { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public List<PhoneNumberModel> PhoneNumbers { get; set; }
}
