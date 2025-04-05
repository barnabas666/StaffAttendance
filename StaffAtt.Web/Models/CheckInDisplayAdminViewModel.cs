using Microsoft.AspNetCore.Mvc.Rendering;
using StaffAttLibrary.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StaffAtt.Web.Models;

/// <summary>
/// ViewModel for displaying list of all CheckIns by Date and Staff for Admin.
/// </summary>
public class CheckInDisplayAdminViewModel
{
    /// <summary>
    /// List of CheckIns.
    /// </summary>
    public List<CheckInFullViewModel> CheckIns {  get; set; } = new List<CheckInFullViewModel>();

    /// <summary>
    /// List of Staff.
    /// </summary>
    public List<StaffBasicViewModel> StaffList { get; set; } = new List<StaffBasicViewModel>();

    /// <summary>
    /// Data for our DropDown control.
    /// Nullable to prevent ModelState validation error in HttpPost Action.
    /// </summary>
    public SelectList? StaffDropDownData { get; set; }

    /// <summary>
    /// Selected Staff's Id from DropDown.
    /// </summary>
    public string SelectedStaffId { get; set; }

    /// <summary>
    /// CheckIn's Start Date for Search Range. Default time is yesterday.
    /// </summary>
    [DataType(DataType.Date)]
    [Required]
    [DisplayName("Start Date")]
    public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-1);

    /// <summary>
    /// CheckIn's End Date for Search Range. Default time is today.
    /// </summary>
    [DataType(DataType.Date)]
    [Required]
    [DisplayName("End Date")]
    public DateTime EndDate { get; set; } = DateTime.Now;
}
