﻿using StaffAttShared.DTOs;
using System.ComponentModel;

namespace StaffAtt.Web.Models;

/// <summary>
/// Staff Model for Details Action View.
/// </summary>
public class StaffDetailsViewModel
{
    /// <summary>
    /// Staff's Basic Info. 
    /// </summary>
    public StaffBasicViewModel BasicInfo { get; set; } = new StaffBasicViewModel();

    /// <summary>
    /// Staff's Address, ViewModel.
    /// </summary>
    public AddressViewModel Address { get; set; } = new AddressViewModel();

    /// <summary>
    /// Staff's Phone Numbers.
    /// </summary>
    [DisplayName("Phone Numbers")]
    public List<PhoneNumberDto> PhoneNumbers { get; set; } = new List<PhoneNumberDto>();

    /// <summary>
    /// Details Action optional parameter. Some info message from different Action.
    /// </summary>
    public string? Message { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}
