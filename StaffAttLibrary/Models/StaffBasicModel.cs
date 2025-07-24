namespace StaffAttLibrary.Models;

/// <summary>
/// Hold Basic Staff data.
/// </summary>
public class StaffBasicModel
{
    /// <summary>
    /// Staff's Id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Staff's First Name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Staff's Last Name.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Staff's Full Name.
    /// </summary>
    public string FullName { get { return FirstName + " " + LastName; } }

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
    /// Department's Id.
    /// </summary>
    public int DepartmentId { get; set; }

    /// <summary>
    /// Staff's Department Title.
    /// </summary>
    public string Title { get; set; }
}
