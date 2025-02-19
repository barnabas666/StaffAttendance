using System.ComponentModel;

namespace StaffAtt.Web.Models;

/// <summary>
/// View Model for Staff Management List View.
/// </summary>
public class StaffManagementListViewModel
{
    /// <summary>
    /// Staff's Basic Info. 
    /// </summary>
    public StaffBasicViewModel BasicInfo { get; set; } = new StaffBasicViewModel();
}
