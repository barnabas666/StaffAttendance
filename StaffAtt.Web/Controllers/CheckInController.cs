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
        CheckInDateDisplayModel dateDisplayModel = new CheckInDateDisplayModel();

        List<CheckInFullModel> checkIns = await _sqlData.GetAllCheckInsByDate(dateDisplayModel.StartDate,
                                                                              dateDisplayModel.EndDate);

        dateDisplayModel.CheckIns = checkIns;

        return View(dateDisplayModel);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> List(CheckInDateDisplayModel dateDisplayModel)
    {
        if (ModelState.IsValid)
        {
            List<CheckInFullModel> checkIns = await _sqlData.GetAllCheckInsByDate(dateDisplayModel.StartDate,
                                                                                  dateDisplayModel.EndDate);

            dateDisplayModel.CheckIns = checkIns;

            return View(dateDisplayModel);
        }

        return RedirectToAction("List");
    }

    public async Task<IActionResult> Display()
    {
        CheckInDateDisplayModel dateDisplayModel = new CheckInDateDisplayModel();

        string userEmail = User.FindFirst(ClaimTypes.Email).Value;

        List<CheckInFullModel> checkIns = await _sqlData.GetCheckInsByDateAndEmail(userEmail,
                                                                                   dateDisplayModel.StartDate,
                                                                                   dateDisplayModel.EndDate);

        dateDisplayModel.CheckIns = checkIns;

        return View(dateDisplayModel);
    }

    [HttpPost]
    public async Task<IActionResult> Display(CheckInDateDisplayModel dateDisplayModel)
    {
        if (ModelState.IsValid)
        {
            string userEmail = User.FindFirst(ClaimTypes.Email).Value;

            List<CheckInFullModel> checkIns = await _sqlData.GetCheckInsByDateAndEmail(userEmail,
                                                                                       dateDisplayModel.StartDate,
                                                                                       dateDisplayModel.EndDate);

            dateDisplayModel.CheckIns = checkIns;

            return View(dateDisplayModel);
        }

        return RedirectToAction("Display");
    }
}
