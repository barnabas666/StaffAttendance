using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Models;

/// <summary>
/// Hold PhoneNumber data. Properties Matchup with PhoneNumbers Table from our database.
/// </summary>
public class PhoneNumberModel
{
    /// <summary>
    /// PhoneNumber's Id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// PhoneNumber's value.
    /// </summary>
    public string PhoneNumber { get; set; }
}
