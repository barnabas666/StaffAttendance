using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StaffAtt.Web.Models;
using StaffAttLibrary.Data;
using StaffAttLibrary.Models;

namespace StaffAtt.Web.Controllers;

/// <summary>
/// Controller for Staff Management, Admin use it.
/// </summary>
[Authorize(Roles = "Administrator")]
public class StaffManagementController : Controller
{
    private readonly IDatabaseData _sqlData;

    public StaffManagementController(IDatabaseData sqlData)
    {
        this._sqlData = sqlData;
    }

    public async Task<IActionResult> List()
    {
        List<StaffBasicModel> staffs = await _sqlData.GetAllBasicStaff();

        return View(staffs);
    }
}
