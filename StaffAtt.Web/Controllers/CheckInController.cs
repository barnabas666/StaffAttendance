using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StaffAtt.Web.Models;
using StaffAttLibrary.Data;
using StaffAttLibrary.Models;
using System.Security.Claims;

namespace StaffAtt.Web.Controllers;

/// <summary>
/// Controller for basic CheckIn's CRUD Actions
/// </summary>
[Authorize]
public class CheckInController : Controller
{
    private readonly IDatabaseData _sqlData;

    public CheckInController(IDatabaseData sqlData)
    {
        _sqlData = sqlData;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> List()
    {
        List<CheckInFullModel> checkIns = await _sqlData.GetAllCheckIns();

        return View(checkIns);
    }

    public async Task<IActionResult> Details()
    {
        string userEmail = User.FindFirst(ClaimTypes.Email).Value;

        List<CheckInFullModel> checkIns = await _sqlData.GetCheckInsByEmail(userEmail);

        return View(checkIns);
    }
}
