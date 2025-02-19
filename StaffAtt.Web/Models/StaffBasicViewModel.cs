using System.ComponentModel;

namespace StaffAtt.Web.Models;

/// <summary>
/// ViewModel for StaffBasicModel.
/// </summary>
public class StaffBasicViewModel
{
    /// <summary>
    /// Staff's Id.
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
    /// Staff's Alias.
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Staff's Approved status.
    /// </summary>
    [DisplayName("Is Approved")]
    public bool IsApproved { get; set; }

    /// <summary>
    /// Department's Id.
    /// </summary>
    public int DepartmentId { get; set; }

    /// <summary>
    /// Staff's Department Title.
    /// </summary>
    [DisplayName("Department")]
    public string Title { get; set; }
}
