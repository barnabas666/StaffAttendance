using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Models;

/// <summary>
/// Hold CheckIn data. Properties Matchup with CheckIns Table from our database.
/// </summary>
public class CheckInModel
{
    /// <summary>
    /// CheckIn's Id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Staff's Id.
    /// </summary>
    public int StaffId { get; set; }

    /// <summary>
    /// CheckIn's Date.
    /// </summary>
    public DateTime CheckInDate { get; set; }

    /// <summary>
    /// CheckOut's Date, possible null value.
    /// </summary>
    public DateTime? CheckOutDate { get; set; }
}
