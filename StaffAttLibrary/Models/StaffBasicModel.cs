﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    /// Staff's Department Description.
    /// </summary>
    public string Description { get; set; }
}
