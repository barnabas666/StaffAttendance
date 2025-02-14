using Microsoft.AspNetCore.Mvc.Rendering;
using StaffAttLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace StaffAtt.Web.Models;

public class CheckInDateDisplayAdminModel
{
    public List<CheckInFullModel> CheckIns {  get; set; } = new List<CheckInFullModel>();

    public List<StaffBasicModel> StaffList { get; set; } = new List<StaffBasicModel>();

    // Data for our DropDown control - Get action
    public SelectList? StaffDropDownData { get; set; }

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
