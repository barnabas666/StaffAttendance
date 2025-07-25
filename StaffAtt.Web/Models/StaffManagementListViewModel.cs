﻿using Microsoft.AspNetCore.Mvc.Rendering;
using StaffAttLibrary.Enums;
using System.ComponentModel.DataAnnotations;

namespace StaffAtt.Web.Models;

/// <summary>
/// View Model for Staff Management List View.
/// </summary>
public class StaffManagementListViewModel
{
    /// <summary>
    /// Staff's Basic Info. 
    /// </summary>
    public List<StaffBasicViewModel> BasicInfos { get; set; } = new List<StaffBasicViewModel>();

    /// <summary>
    /// Department data for our DropDown control
    /// </summary>
    [Display(Name = "Staff's Department: ")]
    public SelectList? DepartmentItems { get; set; }

    /// <summary>
    /// Staff's Department Id.
    /// </summary>    
    public string DepartmentId { get; set; }

    /// <summary>
    /// Enum - Staff's Approved status.
    /// </summary>
    public ApprovedType ApprovedRadio { get; set; }
}
