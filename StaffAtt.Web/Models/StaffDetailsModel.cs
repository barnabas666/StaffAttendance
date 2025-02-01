using StaffAttLibrary.Models;

namespace StaffAtt.Web.Models;

public class StaffDetailsModel
{
    /// <summary>
    /// Staff's First Name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Staff's Last Name.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Staff's Email Address.
    /// </summary>
    public string EmailAddress { get; set; }

    /// <summary>
    /// Staff's Alias.
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Staff's Approved status.
    /// </summary>
    public bool IsApproved { get; set; }

    /// <summary>
    /// Staff's Department Description.
    /// </summary>
    public string Description { get; set; }

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

    /// <summary>
    /// Staff's Phone Numbers.
    /// </summary>
    public List<PhoneNumberModel> PhoneNumbers { get; set; } = new List<PhoneNumberModel>();

    /// <summary>
    /// Details Action optional parameter. Some warning message from different Action, probably from Create.
    /// </summary>
    public string Message { get; set; }
}
