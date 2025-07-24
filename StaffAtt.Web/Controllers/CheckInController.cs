using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StaffAtt.Web.Helpers;
using StaffAtt.Web.Models;
using StaffAttLibrary.Data;
using StaffAttLibrary.Models;

namespace StaffAtt.Web.Controllers;

/// <summary>
/// Controller for basic CheckIn's CRUD Actions
/// </summary>
[Authorize]
public class CheckInController : Controller
{
    private readonly IStaffService _staffService;
    private readonly ICheckInService _checkInService;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IStaffSelectListService _staffSelectListService;

    public CheckInController(IStaffService staffService,
                             ICheckInService checkInService,
                             IUserContext userContext,
                             IMapper mapper,
                             IStaffSelectListService staffSelectListService)
    {
        _staffService = staffService;
        _checkInService = checkInService;
        _userContext = userContext;
        _mapper = mapper;
        _staffSelectListService = staffSelectListService;
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

        List<CheckInFullModel> checkIns = await _checkInService.GetAllCheckInsByDateAsync(dateDisplayModel.StartDate,
                                                                              dateDisplayModel.EndDate);
        dateDisplayModel.CheckIns = _mapper.Map<List<CheckInFullViewModel>>(checkIns);

        List<StaffBasicModel> basicStaff = await _staffService.GetAllBasicStaffAsync();
        dateDisplayModel.StaffList = _mapper.Map<List<StaffBasicViewModel>>(basicStaff);

        dateDisplayModel.StaffDropDownData = await _staffSelectListService.GetStaffSelectListAsync(dateDisplayModel,
                                                                                  "All Staff");

        return View("List", dateDisplayModel);
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
        List<CheckInFullModel> checkIns = new List<CheckInFullModel>();

        if (dateDisplayModel.SelectedStaffId == "0")
        {
            checkIns = await _checkInService.GetAllCheckInsByDateAsync(dateDisplayModel.StartDate,
                                                                  dateDisplayModel.EndDate);
            dateDisplayModel.CheckIns = _mapper.Map<List<CheckInFullViewModel>>(checkIns);
        }
        else
        {
            checkIns = await _checkInService.GetCheckInsByDateAndIdAsync(Convert.ToInt32(dateDisplayModel.SelectedStaffId),
                                                                             dateDisplayModel.StartDate,
                                                                             dateDisplayModel.EndDate);
            dateDisplayModel.CheckIns = _mapper.Map<List<CheckInFullViewModel>>(checkIns);
        }

        List<StaffBasicModel> basicStaff = await _staffService.GetAllBasicStaffAsync();
        dateDisplayModel.StaffList = _mapper.Map<List<StaffBasicViewModel>>(basicStaff);

        dateDisplayModel.StaffDropDownData = await _staffSelectListService.GetStaffSelectListAsync(dateDisplayModel,
                                                                                  "All Staff");

        return View("List", dateDisplayModel);
    }

    /// <summary>
    /// Display list of all CheckIns by Date and Staff for given Staff.
    /// First populate ViewModel with data from Db for given Staff and send to View.
    /// </summary>
    /// <returns>ViewModel with populated CheckInDateDisplayStaffModel.</returns>
    public async Task<IActionResult> Display()
    {
        string userEmail = _userContext.GetUserEmail();

        CheckInDisplayStaffViewModel dateDisplayModel = new CheckInDisplayStaffViewModel();

        List<CheckInFullModel> checkIns = await _checkInService.GetCheckInsByDateAndEmailAsync(userEmail,
                                                                                   dateDisplayModel.StartDate,
                                                                                   dateDisplayModel.EndDate);
        dateDisplayModel.CheckIns = _mapper.Map<List<CheckInFullViewModel>>(checkIns);

        return View("Display", dateDisplayModel);
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
        string userEmail = _userContext.GetUserEmail();

        List<CheckInFullModel> checkIns = await _checkInService.GetCheckInsByDateAndEmailAsync(userEmail,
                                                                             dateDisplayModel.StartDate,
                                                                             dateDisplayModel.EndDate);
        dateDisplayModel.CheckIns = _mapper.Map<List<CheckInFullViewModel>>(checkIns);

        return View("Display", dateDisplayModel);
    }
}
