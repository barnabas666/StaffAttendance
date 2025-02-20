using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StaffAtt.Web.Models;

public class AddressViewModel
{
    /// <summary>
    /// Staff's Address - Street.
    /// </summary>
    [Required(ErrorMessage = "Please enter an Address - Street.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Street must be between 2 and 50 chars.")]
    public string Street { get; set; }

    /// <summary>
    /// Staff's Address - City.
    /// </summary>
    [Required(ErrorMessage = "Please enter an Address - City.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "City must be between 2 and 50 chars.")]
    public string City { get; set; }

    /// <summary>
    /// Staff's Address - Zip.
    /// </summary>
    [Required(ErrorMessage = "Please enter an Address - Zip.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Zip must be between 6 and 10 chars.")]
    [DisplayName("Zip Code")]
    public string Zip { get; set; }

    /// <summary>
    /// Staff's Address - State.
    /// </summary>
    [Required(ErrorMessage = "Please enter an Address - State.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "State must be between 2 and 50 chars.")]
    public string State { get; set; }
}
