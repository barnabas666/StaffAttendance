using System.ComponentModel;

namespace StaffAtt.Web.Models;

public class AddressViewModel
{
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
    [DisplayName("Zip Code")]
    public string Zip { get; set; }

    /// <summary>
    /// Staff's Address - State.
    /// </summary>
    public string State { get; set; }
}
