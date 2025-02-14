using Microsoft.AspNetCore.Mvc.Rendering;
using StaffAttLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace StaffAtt.Web.Models;

/// <summary>
/// ViewModel for displaying list of all CheckIns by Date and Staff for Admin.
/// </summary>
public class CheckInDateDisplayAdminModel
{
    /// <summary>
    /// List of CheckIns.
    /// </summary>
    public List<CheckInFullModel> CheckIns {  get; set; } = new List<CheckInFullModel>();

    /// <summary>
    /// List of Staff.
    /// </summary>
    public List<StaffBasicModel> StaffList { get; set; } = new List<StaffBasicModel>();

    /// <summary>
    /// Data for our DropDown control.
    /// Nullable to prevent ModelState validation error in HttpPost Action.
    /// </summary>
    public SelectList? StaffDropDownData { get; set; }

    /// <summary>
    /// Selected Staff's Id from DropDown.
    /// </summary>
    public string SelectedId { get; set; }

    /// <summary>
    /// CheckIn's Start Date for Search Range. Default time is yesterday.
    /// </summary>
    [DataType(DataType.Date)]
    [Required]
    public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-1);

    /// <summary>
    /// CheckIn's End Date for Search Range. Default time is today.
    /// </summary>
    [DataType(DataType.Date)]
    [Required]
    public DateTime EndDate { get; set; } = DateTime.Now;
}
