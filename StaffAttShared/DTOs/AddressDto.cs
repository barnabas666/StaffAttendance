namespace StaffAttShared.DTOs;
public class AddressDto
{
    /// <summary>
    /// Addresses Id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Staff's Address - Street.
    /// </summary>
    public string Street { get; set; }

    /// <summary>
    /// Staff's Address - City.
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Staff's Address - Zip.
    /// </summary>
    public string Zip { get; set; }

    /// <summary>
    /// Staff's Address - State.
    /// </summary>
    public string State { get; set; }
}
