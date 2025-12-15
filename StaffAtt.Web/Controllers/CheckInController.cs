using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StaffAtt.Web.Models;
using StaffAtt.Web.Services;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Controllers;

/// <summary>
/// Controller for basic CheckIn's CRUD Actions
/// </summary>
[Authorize]
public class CheckInController : Controller
{
    private readonly IApiClient _apiClient;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IStaffSelectListService _staffSelectListService;

    public CheckInController(IApiClient apiClient,
                             IUserContext userContext,
                             IMapper mapper,
                             IStaffSelectListService staffSelectListService)
    {
        _apiClient = apiClient;
        _userContext = userContext;
        _mapper = mapper;
        _staffSelectListService = staffSelectListService;
    }

    /// <summary>
    /// Displays list of all CheckIns by Date and Staff for Admin (initial load).
    /// </summary>
    [Authorize(Roles = "Administrator")]
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var viewModel = new CheckInDisplayAdminViewModel();

        // --- Load all check-ins by date range ---
        var checkInResult = await _apiClient.GetAsync<List<CheckInFullDto>>(
            $"checkin/all?startDate={viewModel.StartDate:yyyy-MM-dd}&endDate={viewModel.EndDate:yyyy-MM-dd}"
        );

        if (!checkInResult.IsSuccess)
            return View("Error", new ErrorViewModel { Message = checkInResult.ErrorMessage });

        viewModel.CheckIns = _mapper.Map<List<CheckInFullViewModel>>(checkInResult.Value);

        // --- Load staff for dropdown ---
        var staffResult = await _apiClient.GetAsync<List<StaffBasicDto>>("staff/basic");
        if (!staffResult.IsSuccess)
            return View("Error", new ErrorViewModel { Message = staffResult.ErrorMessage });

        viewModel.StaffList = _mapper.Map<List<StaffBasicViewModel>>(staffResult.Value);
        viewModel.StaffDropDownData = await _staffSelectListService.GetStaffSelectListAsync(viewModel, "All Staff");

        return View("List", viewModel);
    }

    /// <summary>
    /// Handles filter requests for CheckIns (by date and staff).
    /// </summary>
    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> List(CheckInDisplayAdminViewModel viewModel)
    {
        // Determine which API endpoint to use
        string endpoint;

        if (viewModel.SelectedStaffId == 0)
            endpoint = $"checkin/all?startDate={viewModel.StartDate:yyyy-MM-dd}&endDate={viewModel.EndDate:yyyy-MM-dd}";
        else
            endpoint = $"checkin/byId/{viewModel.SelectedStaffId}?startDate={viewModel.StartDate:yyyy-MM-dd}&endDate={viewModel.EndDate:yyyy-MM-dd}";

        var checkInResult = await _apiClient.GetAsync<List<CheckInFullDto>>(endpoint);
        if (!checkInResult.IsSuccess)
            return View("Error", new ErrorViewModel { Message = checkInResult.ErrorMessage });

        viewModel.CheckIns = _mapper.Map<List<CheckInFullViewModel>>(checkInResult.Value);

        // Reload dropdown + staff list
        var staffResult = await _apiClient.GetAsync<List<StaffBasicDto>>("staff/basic");
        if (!staffResult.IsSuccess)
            return View("Error", new ErrorViewModel { Message = staffResult.ErrorMessage });

        viewModel.StaffList = _mapper.Map<List<StaffBasicViewModel>>(staffResult.Value);
        viewModel.StaffDropDownData = await _staffSelectListService.GetStaffSelectListAsync(viewModel, "All Staff");

        return View("List", viewModel);
    }

    /// <summary>
    /// Display list of all CheckIns by Date and Staff for the currently logged-in staff member.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Display()
    {
        string userEmail = _userContext.GetUserEmail();

        var viewModel = new CheckInDisplayStaffViewModel();

        // Call API endpoint: GET /api/checkin/byEmail?emailAddress=...&startDate=...&endDate=...
        var result = await _apiClient.GetAsync<List<CheckInFullDto>>(
            $"checkin/byEmail?emailAddress={Uri.EscapeDataString(userEmail)}" +
            $"&startDate={viewModel.StartDate:yyyy-MM-dd}&endDate={viewModel.EndDate:yyyy-MM-dd}"
        );

        if (!result.IsSuccess)
            return View("Error", new ErrorViewModel { Message = result.ErrorMessage });

        viewModel.CheckIns = _mapper.Map<List<CheckInFullViewModel>>(result.Value);

        return View("Display", viewModel);
    }

    /// <summary>
    /// HttpPost Action for displaying list of CheckIns for given Staff within date range.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Display(CheckInDisplayStaffViewModel viewModel)
    {
        string userEmail = _userContext.GetUserEmail();

        var result = await _apiClient.GetAsync<List<CheckInFullDto>>(
            $"checkin/byEmail?emailAddress={Uri.EscapeDataString(userEmail)}" +
            $"&startDate={viewModel.StartDate:yyyy-MM-dd}&endDate={viewModel.EndDate:yyyy-MM-dd}"
        );

        if (!result.IsSuccess)
            return View("Error", new ErrorViewModel { Message = result.ErrorMessage });

        viewModel.CheckIns = _mapper.Map<List<CheckInFullViewModel>>(result.Value);

        return View("Display", viewModel);
    }
}
