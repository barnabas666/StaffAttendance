using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Models;

/// <summary>
/// Hold Staff-PhoneNumber's data. Properties Matchup with StaffPhoneNumbers Table from our database.
/// </summary>
public class StaffPhoneNumberModel
{
    /// <summary>
    /// Staff-PhoneNumber's relation Table Id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Staff's Id.
    /// </summary>
    public int StaffId { get; set; }

    /// <summary>
    /// PhoneNumber's Id.
    /// </summary>
    public int PhoneNumberId { get; set; }
}
