using System.ComponentModel;

namespace StaffAtt.Web.Models;

public class CheckInFullViewModel
{
    /// <summary>
    /// CheckIn's Id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Staff's First Name.
    /// </summary>
    [DisplayName("First Name")]
    public string FirstName { get; set; }

    /// <summary>
    /// Staff's Last Name.
    /// </summary>
    [DisplayName("Last Name")]
    public string LastName { get; set; }

    /// <summary>
    /// Staff's Full Name.
    /// </summary>
    [DisplayName("Full Name")]
    public string FullName { get { return FirstName + " " + LastName; } }

    /// <summary>
    /// Staff's Email Address.
    /// </summary>
    [DisplayName("Email Address")]
    public string EmailAddress { get; set; }

    /// <summary>
    /// Staff's Department Title.
    /// </summary>
    [DisplayName("Department")]
    public string Title { get; set; }

    /// <summary>
    /// CheckIn's Date.
    /// </summary>
    [DisplayName("Start Date")]
    public DateTime CheckInDate { get; set; }

    /// <summary>
    /// CheckOut's Date, possible null value.
    /// </summary>
    [DisplayName("End Date")]
    public DateTime? CheckOutDate { get; set; }
}
