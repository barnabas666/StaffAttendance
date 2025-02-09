﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Models;

/// <summary>
/// Hold Full CheckIn data - StaffBasicModel and CheckInModel.
/// </summary>
public class CheckInFullModel
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
    /// Staff's Department Title.
    /// </summary>
    public string Title { get; set; }

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
