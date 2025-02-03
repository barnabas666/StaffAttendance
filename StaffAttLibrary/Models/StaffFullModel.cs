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
public class StaffFullModel : StaffBasicModel
{
    /// <summary>
    /// Addresses Id.
    /// </summary>
    public int AddressId { get; set; }

    /// <summary>
    /// Aliases Id.
    /// </summary>
    public int AliasId { get; set; }

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
}
