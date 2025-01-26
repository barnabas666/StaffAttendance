using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Models;

/// <summary>
/// Hold Full Staff data. Properties Matchup with Staffs, Departments, Addresses, 
/// Aliases and PhoneNumbers Tables from our database.
/// </summary>
public class StaffFullModel
{
    /// <summary>
    /// Staff's Id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Department's Id.
    /// </summary>
    public int DepartmentId { get; set; }

    /// <summary>
    /// Addresses Id.
    /// </summary>
    public int AddressId { get; set; }

    /// <summary>
    /// Aliases Id.
    /// </summary>
    public int AliasId { get; set; }

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
    /// Staff's Approved status.
    /// </summary>
    public bool IsApproved { get; set; }

    /// <summary>
    /// Staff's Title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Staff's Description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Staff's Street.
    /// </summary>
    public string Street { get; set; }

    /// <summary>
    /// Staff's City.
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Staff's Zip.
    /// </summary>
    public string Zip { get; set; }

    /// <summary>
    /// Staff's State.
    /// </summary>
    public string State { get; set; }

    /// <summary>
    /// Staff's Alias.
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Staff's Phone Numbers.
    /// </summary>
    public List<PhoneNumberModel> PhoneNumbers { get; set; } = new List<PhoneNumberModel>();
}
