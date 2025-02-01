using Microsoft.AspNetCore.Mvc.Rendering;
using StaffAttLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace StaffAtt.Web.Models;

/// <summary>
/// Model needed to send to View (Create Action), populate it there and send model back.
/// </summary>
public class StaffCreateModel
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
    /// Department data for our DropDown control - Get action
    /// </summary>
    [Display(Name = "Your Department: ")]    
    public SelectList? DepartmentItems { get; set; }

    /// <summary>
    /// Staff's Department Id.
    /// </summary>
    [Required]
    public string DepartmentId { get; set; }

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
    public string Zip { get; set; }

    /// <summary>
    /// Staff's Address - State.
    /// </summary>
    [Required(ErrorMessage = "Please enter an Address - State.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "State must be between 2 and 50 chars.")]
    public string State { get; set; }

    /// <summary>
    /// Staff's Phone Numbers.
    /// </summary>
    [Display(Name = "Phone Numbers")]
    [Required(ErrorMessage = "Please enter one or more Phone Numbers seperated by comma, example: 123456789,111222333,999888777")]
    [StringLength(100, MinimumLength = 9, ErrorMessage = "Please enter at least one Phone Number.")]
    public string PhoneNumbersText { get; set; }
}
