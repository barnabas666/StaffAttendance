using AutoMapper;
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
    private readonly IStaffData _staffData;
    private readonly ICheckInData _checkInData;
    private readonly IMapper _mapper;

    public CheckInController(IStaffData staffData, ICheckInData checkInData, IMapper mapper)
    {
        _staffData = staffData;
        _checkInData = checkInData;
        _mapper = mapper;
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
        CheckInDisplayAdminViewModel dateDisplayModel = new CheckInDisplayAdminViewModel();

        List<CheckInFullModel> checkIns = await _checkInData.GetAllCheckInsByDate(dateDisplayModel.StartDate,
                                                                              dateDisplayModel.EndDate);
        dateDisplayModel.CheckIns = _mapper.Map<List<CheckInFullViewModel>>(checkIns);

        List<StaffBasicModel> basicStaff = await _staffData.GetAllBasicStaff();
        dateDisplayModel.StaffList = _mapper.Map<List<StaffBasicViewModel>>(basicStaff);

        // Creating default item = All Staff for DropDown. FullName prop is just getter so we setup here FirstName.
        dateDisplayModel.StaffList.Insert(0, new StaffBasicViewModel()
                                                        {
                                                            Id = 0,
                                                            FirstName = "All",
                                                            LastName = "Staff"
                                                        });

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
    public async Task<IActionResult> List(CheckInDisplayAdminViewModel dateDisplayModel)
    {
        if (ModelState.IsValid == false)        
            return RedirectToAction("List");
        
        List<CheckInFullModel> checkIns = new List<CheckInFullModel>();

        if (dateDisplayModel.SelectedId == "0")
        {
            checkIns = await _checkInData.GetAllCheckInsByDate(dateDisplayModel.StartDate,
                                                           dateDisplayModel.EndDate);
            dateDisplayModel.CheckIns = _mapper.Map<List<CheckInFullViewModel>>(checkIns);
        }
        else
        {
            checkIns = await _checkInData.GetCheckInsByDateAndId(Convert.ToInt32(dateDisplayModel.SelectedId),
                                                                             dateDisplayModel.StartDate,
                                                                             dateDisplayModel.EndDate);
            dateDisplayModel.CheckIns = _mapper.Map<List<CheckInFullViewModel>>(checkIns);
        }

        List<StaffBasicModel> basicStaff = await _staffData.GetAllBasicStaff();
        dateDisplayModel.StaffList = _mapper.Map<List<StaffBasicViewModel>>(basicStaff);

        // Creating default item = All Staff for DropDown. FullName prop is just getter so we setup here FirstName.
        dateDisplayModel.StaffList.Insert(0, new StaffBasicViewModel()
                                                        {
                                                            Id = 0,
                                                            FirstName = "All",
                                                            LastName = "Staff"
                                                        });

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
        CheckInDisplayStaffViewModel dateDisplayModel = new CheckInDisplayStaffViewModel();

        string userEmail = User.FindFirst(ClaimTypes.Email).Value;

        List<CheckInFullModel> checkIns = await _checkInData.GetCheckInsByDateAndEmail(userEmail,
                                                                                   dateDisplayModel.StartDate,
                                                                                   dateDisplayModel.EndDate);
        dateDisplayModel.CheckIns = _mapper.Map<List<CheckInFullViewModel>>(checkIns);

        return View(dateDisplayModel);
    }

    /// <summary>
    /// HttpPost Action for displaying of list of all CheckIns by Date and Staff for given Staff.
    /// After hit Submit button we get data from Db, repopulate ViewModel and send back to View.
    /// </summary>
    /// <param name="dateDisplayModel">ViewModel</param>
    /// <returns>ViewModel with repopulated CheckInDateDisplayStaffModel.</returns>
    [HttpPost]
    public async Task<IActionResult> Display(CheckInDisplayStaffViewModel dateDisplayModel)
    {
        if (ModelState.IsValid == false)        
            return RedirectToAction("Display");
                
        string userEmail = User.FindFirst(ClaimTypes.Email).Value;

        List<CheckInFullModel> checkIns = await _checkInData.GetCheckInsByDateAndEmail(userEmail,
                                                                             dateDisplayModel.StartDate,
                                                                             dateDisplayModel.EndDate);
        dateDisplayModel.CheckIns = _mapper.Map<List<CheckInFullViewModel>>(checkIns); 

        return View(dateDisplayModel);
    }
}
