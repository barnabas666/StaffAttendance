namespace StaffAttShared.DTOs;

public class StaffFullDto : StaffBasicDto
{
    /// <summary>
    /// Addresses Id.
    /// </summary>
    public int AddressId { get; set; }

    /// <summary>
    /// Aliases Id.
    /// </summary>
    public int AliasId { get; set; }

    /// <summary>
    /// Staff's Department Description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Staff's Address.
    /// </summary>
    public AddressDto Address { get; set; } = new AddressDto();

    /// <summary>
    /// Staff's Phone Numbers.
    /// </summary>
    public List<PhoneNumberDto> PhoneNumbers { get; set; } = new List<PhoneNumberDto>();
}
