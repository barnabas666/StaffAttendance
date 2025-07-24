using System.ComponentModel.DataAnnotations;

namespace StaffAtt.Web.Models;

/// <summary>
/// ViewModel for displaying list of all CheckIns by Date and Staff for given Staff.
/// </summary>
public class CheckInDisplayStaffViewModel
{
    /// <summary>
    /// List of CheckIns.
    /// </summary>
    public List<CheckInFullViewModel> CheckIns { get; set; } = new List<CheckInFullViewModel>();

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
