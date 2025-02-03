﻿using Microsoft.AspNetCore.Mvc.Rendering;
using StaffAttLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace StaffAtt.Web.Models;

/// <summary>
/// Model needed to send to View (Update Action), populate it there and send model back.
/// </summary>
public class StaffManagementUpdateModel
{
    /// <summary>
    /// Staff's Basic Info.
    /// </summary>
    public StaffBasicModel BasicInfo { get; set; }

    /// <summary>
    /// Department data for our DropDown control - Get action
    /// </summary>
    [Display(Name = "Staff's Department: ")]
    public SelectList? DepartmentItems { get; set; }
}
