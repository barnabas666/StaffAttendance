namespace StaffAtt.Web.Models;

/// <summary>
/// View Model for Delete Action in Staff Management Controller.
/// </summary>
public class StaffManagementDeleteViewModel
{
    /// <summary>
    /// Staff's Basic Info. 
    /// </summary>
    public StaffBasicViewModel BasicInfo { get; set; } = new StaffBasicViewModel();
}
