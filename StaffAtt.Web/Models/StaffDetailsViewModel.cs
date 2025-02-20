using StaffAttLibrary.Models;
using System.ComponentModel;

namespace StaffAtt.Web.Models;

/// <summary>
/// Staff Model for Details Action View.
/// </summary>
public class StaffDetailsViewModel 
{
    /// <summary>
    /// Staff's Basic Info. 
    /// </summary>
    public StaffBasicViewModel BasicInfo { get; set; } = new StaffBasicViewModel();

    /// <summary>
    /// Staff's Address, ViewModel.
    /// </summary>
    public AddressViewModel Address { get; set; } = new AddressViewModel();

    /// <summary>
    /// Staff's Phone Numbers.
    /// </summary>
    [DisplayName("Phone Numbers")]
    public List<PhoneNumberModel> PhoneNumbers { get; set; } = new List<PhoneNumberModel>();

    /// <summary>
    /// Details Action optional parameter. Some warning message from different Action, probably from Create.
    /// </summary>
    public string Message { get; set; }
}
