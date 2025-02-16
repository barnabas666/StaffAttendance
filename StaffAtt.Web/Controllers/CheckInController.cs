using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    /// <summary>
    /// Display list of all CheckIns by Date and Staff for Admin.
    /// First populate ViewModel with data from Db and send to View.
    /// </summary>
    /// <returns>ViewModel with populated CheckInDateDisplayAdminModel.</returns>
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> List()
    {
        CheckInDateDisplayAdminModel dateDisplayModel = new CheckInDateDisplayAdminModel();

        dateDisplayModel.CheckIns = await _sqlData.GetAllCheckInsByDate(dateDisplayModel.StartDate,
                                                                      dateDisplayModel.EndDate);

        dateDisplayModel.StaffList = await _sqlData.GetAllBasicStaff();
        // Creating default item = All Staff for DropDown. FullName prop is just getter so we setup here FirstName.
        dateDisplayModel.StaffList.Insert(0, new StaffBasicModel() { Id = 0, FirstName = "All Staff" });

        // Source is StaffList, value (Id here) gonna be saved to database, Text (FirstName) gets displayed to user.
        dateDisplayModel.StaffDropDownData = new SelectList(dateDisplayModel.StaffList,
                                                            nameof(StaffBasicModel.Id),
                                                            nameof(StaffBasicModel.FullName));
        
        return View(dateDisplayModel);
    }

    /// <summary>
    /// HttpPost Action for displaying of list of all CheckIns by Date and Staff for Admin.
    /// After hit Submit button or changing of Staff in DropDown, we get data from Db,
    /// repopulate ViewModel and send back to View.
    /// </summary>
    /// <param name="dateDisplayModel">ViewModel</param>
    /// <returns>ViewModel with repopulated CheckInDateDisplayAdminModel.</returns>
    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> List(CheckInDateDisplayAdminModel dateDisplayModel)
    {
        if (ModelState.IsValid == false)        
            return RedirectToAction("List");
        
        List<CheckInFullModel> checkIns = new List<CheckInFullModel>();

        if (dateDisplayModel.SelectedId == "0")
        {
            dateDisplayModel.CheckIns = await _sqlData.GetAllCheckInsByDate(dateDisplayModel.StartDate,
                                                           dateDisplayModel.EndDate);
        }
        else
        {
            dateDisplayModel.CheckIns = await _sqlData.GetCheckInsByDateAndId(Convert.ToInt32(dateDisplayModel.SelectedId),
                                                                             dateDisplayModel.StartDate,
                                                                             dateDisplayModel.EndDate);
        }        

        dateDisplayModel.StaffList = await _sqlData.GetAllBasicStaff();
        // Creating default item = All Staff for DropDown. FullName prop is just getter so we setup here FirstName.
        dateDisplayModel.StaffList.Insert(0, new StaffBasicModel() { Id = 0, FirstName = "All Staff" });

        // Source is StaffList, value (Id here) gonna be saved to database, Text (FirstName) gets displayed to user.
        dateDisplayModel.StaffDropDownData = new SelectList(dateDisplayModel.StaffList,
                                                            nameof(StaffBasicModel.Id),
                                                            nameof(StaffBasicModel.FullName));

        return View(dateDisplayModel);
    }

    /// <summary>
    /// Display list of all CheckIns by Date and Staff for given Staff.
    /// First populate ViewModel with data from Db for given Staff and send to View.
    /// </summary>
    /// <returns>ViewModel with populated CheckInDateDisplayStaffModel.</returns>
    public async Task<IActionResult> Display()
    {
        CheckInDateDisplayStaffModel dateDisplayModel = new CheckInDateDisplayStaffModel();

        string userEmail = User.FindFirst(ClaimTypes.Email).Value;

        dateDisplayModel.CheckIns = await _sqlData.GetCheckInsByDateAndEmail(userEmail,
                                                                             dateDisplayModel.StartDate,
                                                                             dateDisplayModel.EndDate);

        return View(dateDisplayModel);
    }

    /// <summary>
    /// HttpPost Action for displaying of list of all CheckIns by Date and Staff for given Staff.
    /// After hit Submit button we get data from Db, repopulate ViewModel and send back to View.
    /// </summary>
    /// <param name="dateDisplayModel">ViewModel</param>
    /// <returns>ViewModel with repopulated CheckInDateDisplayStaffModel.</returns>
    [HttpPost]
    public async Task<IActionResult> Display(CheckInDateDisplayStaffModel dateDisplayModel)
    {
        if (ModelState.IsValid == false)        
            return RedirectToAction("Display");
                
        string userEmail = User.FindFirst(ClaimTypes.Email).Value;

        dateDisplayModel.CheckIns = await _sqlData.GetCheckInsByDateAndEmail(userEmail,
                                                                             dateDisplayModel.StartDate,
                                                                             dateDisplayModel.EndDate);

        return View(dateDisplayModel);
    }
}
