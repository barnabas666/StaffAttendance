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
    /// Staff's Address.
    /// </summary>
    public AddressModel Address { get; set; } = new AddressModel();

    /// <summary>
    /// Staff's Phone Numbers.
    /// </summary>
    public List<PhoneNumberModel> PhoneNumbers { get; set; } = new List<PhoneNumberModel>();
}
