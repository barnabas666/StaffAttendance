using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StaffAtt.Web.Models;

/// <summary>
/// Model needed to send to View (Update Action), populate it there and send model back.
/// </summary>
public class StaffUpdateViewModel
{
    /// <summary>
    /// Staff's First Name.
    /// </summary>
    [Display(Name = "First Name")]
    [Required(ErrorMessage = "Please enter a First Name.")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 30 chars.")]
    public string FirstName { get; set; }

    /// <summary>
    /// Staff's Last Name.
    /// </summary>
    [Display(Name = "Last Name")]
    [Required(ErrorMessage = "Please enter a Last Name.")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 and 30 chars.")]
    public string LastName { get; set; }

    /// <summary>
    /// Staff's PIN. To Db will be saved as string.
    /// </summary>
    [Required(ErrorMessage = "Please enter a PIN.")]
    [Range(1000, 9999, ErrorMessage = "PIN must contain exactly 4 digits.")]
    public int PIN { get; set; }

    /// <summary>
    /// Staff's Address, ViewModel.
    /// </summary>
    public AddressViewModel Address { get; set; }

    /// <summary>
    /// Staff's Email. Hidden field.
    /// </summary>
    public string EmailAddress { get; set; }

    /// <summary>
    /// Staff's Phone Numbers.
    /// </summary>
    [Display(Name = "Phone Numbers")]
    [Required(ErrorMessage = "Please enter one or more Phone Numbers seperated by comma, example: 123456789,111222333,999888777")]
    [StringLength(100, MinimumLength = 9, ErrorMessage = "Please enter at least one Phone Number.")]
    public string PhoneNumbersText { get; set; }
}
