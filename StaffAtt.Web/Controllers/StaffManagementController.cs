using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StaffAtt.Web.Models;
using StaffAtt.Web.Services;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Controllers;

/// <summary>
/// Controller for Staff Management, Admin use it.
/// </summary>
[Authorize(Roles = "Administrator")]
public class StaffManagementController : Controller
{
    private readonly IApiClient _apiClient;
    private readonly IAuthClient _authClient;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IDepartmentSelectListService _departmentService;

    public StaffManagementController(IApiClient apiClient,
                                     IAuthClient authClient,
                                     IUserService userService,
                                     IMapper mapper,
                                     IDepartmentSelectListService departmentService)
    {
        _apiClient = apiClient;
        _authClient = authClient;
        _userService = userService;
        _mapper = mapper;
        _departmentService = departmentService;
    }

    /// <summary>
    /// Retrieves and displays a list of staff members along with department options.
    /// </summary>
    /// <remarks>This method fetches basic information about all staff members and maps it to a view model. 
    /// It also retrieves a list of departments for selection purposes. The resulting data is passed  to the view for
    /// rendering. Display Check-In Approval status for all staff.</remarks>
    /// <returns>An <see cref="IActionResult"/> that renders the staff management list view with the populated data.</returns>
    [HttpGet]
    public async Task<IActionResult> List()
    {
        StaffManagementListViewModel staffModel = new StaffManagementListViewModel();

        var result = await _apiClient.GetAsync<List<StaffBasicDto>>("staff/basic");
        if (!result.IsSuccess || result.Value is null)
            return View("Error", new ErrorViewModel { Message = result.ErrorMessage });

        staffModel.BasicInfos = _mapper.Map<List<StaffBasicViewModel>>(result.Value);
        staffModel.DepartmentItems = await _departmentService.GetDepartmentSelectListAsync("All Departments");

        return View("List", staffModel);
    }

    /// <summary>
    /// Retrieves a filtered list of staff members and populates the provided view model with the results.
    /// </summary>
    /// <remarks>The method filters staff members based on the department ID and approval status provided in
    /// the  <paramref name="staffModel"/>. It also populates the view model with a list of department options for
    /// display in the view.</remarks>
    /// <param name="staffModel">The view model containing filter criteria, such as department ID and approval status,  and used to store the
    /// resulting staff information and department options.</param>
    /// <returns>An <see cref="IActionResult"/> that renders the view with the populated <paramref name="staffModel"/>.</returns>
    [HttpPost]
    public async Task<IActionResult> List(StaffManagementListViewModel staffModel)
    {
        // Call API endpoint with query parameters
        var result = await _apiClient.GetAsync<List<StaffBasicDto>>(
            $"staff/basic/filter?departmentId={staffModel.DepartmentId}&approvedType={(int)staffModel.ApprovedRadio}"
        );
        if (!result.IsSuccess || result.Value is null)
            return View("Error", new ErrorViewModel { Message = result.ErrorMessage });

        staffModel.BasicInfos = _mapper.Map<List<StaffBasicViewModel>>(result.Value);
        staffModel.DepartmentItems = await _departmentService.GetDepartmentSelectListAsync("All Departments");

        return View("List", staffModel);
    }

    /// <summary>
    /// Retrieves the complete details of a staff member by their unique identifier.
    /// </summary>
    /// <remarks>This method asynchronously retrieves the staff member's details using the provided
    /// identifier, maps the data to a view model, and returns a view for rendering. Ensure that the <paramref
    /// name="id"/> corresponds to an existing staff member in the system.</remarks>
    /// <param name="id">The unique identifier of the staff member to retrieve.</param>
    /// <returns>An <see cref="IActionResult"/> that renders the details view of the staff member.</returns>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        // Call API to get staff by ID
        var result = await _apiClient.GetAsync<StaffFullDto>($"staff/{id}");
        if (!result.IsSuccess || result.Value is null)
            return View("Error", new ErrorViewModel { Message = result.ErrorMessage });

        var detailsModel = _mapper.Map<StaffDetailsViewModel>(result.Value);

        return View("Details", detailsModel);
    }

    /// <summary>
    /// Displays the update view for a staff member with the specified ID, only for admin.
    /// </summary>
    /// <remarks>Admin can change Approval status and Department. 
    /// This method retrieves the basic details of the staff member using the provided ID, maps them
    /// to a view model,  and populates the department selection list for display in the update view.</remarks>
    /// <param name="id">The unique identifier of the staff member to update.</param>
    /// <returns>An <see cref="IActionResult"/> that renders the update view populated with the staff member's details  and a
    /// list of available departments.</returns>
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var result = await _apiClient.GetAsync<StaffBasicDto>($"staff/basic/{id}");
        if (!result.IsSuccess || result.Value is null)
            return View("Error", new ErrorViewModel { Message = result.ErrorMessage });

        var updateModel = _mapper.Map<StaffManagementUpdateViewModel>(result.Value);
        updateModel.DepartmentItems = await _departmentService.GetDepartmentSelectListAsync(String.Empty);

        return View("Update", updateModel);
    }

    [HttpPost]
    public async Task<IActionResult> Update(StaffManagementUpdateViewModel updateModel)
    {
        var request = new UpdateStaffByAdminRequest
        {
            Id = updateModel.BasicInfo.Id,
            DepartmentId = Convert.ToInt32(updateModel.BasicInfo.DepartmentId),
            IsApproved = updateModel.BasicInfo.IsApproved
        };

        var createResult = await _apiClient.PutAsync<UpdateStaffByAdminRequest>("staff/admin", request);
        if (!createResult.IsSuccess)
            return View("Error", new ErrorViewModel { Message = createResult.ErrorMessage });

        TempData["Success"] = $"{updateModel.BasicInfo.FirstName} {updateModel.BasicInfo.LastName} profile was successfully updated.";
        return RedirectToAction("List");
    }

    /// <summary>
    /// Displays the delete confirmation view for a staff member with the specified ID.
    /// </summary>
    /// <remarks>We get StaffBasicModel by Id (passed in URL from List Page after we hit Delete button)<\remarks>
    /// <param name="id">The unique identifier of the staff member to delete.</param>
    /// <returns>An <see cref="IActionResult"/> that renders the delete confirmation view populated with the staff member's
    /// details.</returns>
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _apiClient.GetAsync<StaffBasicDto>($"staff/basic/{id}");
        if (!result.IsSuccess || result.Value is null)
            return View("Error", new ErrorViewModel { Message = result.ErrorMessage });

        StaffManagementDeleteViewModel deleteModel = _mapper.Map<StaffManagementDeleteViewModel>(result.Value);

        // return View with our Staff Info to delete.
        return View("Delete", deleteModel);
    }

    /// <summary>
    /// Deletes a staff member and their associated user account.
    /// </summary>
    /// <remarks>This method performs the following actions: Retrieves
    /// the email address of the staff member using their ID. Deletes the staff
    /// member from the system. Finds and deletes the associated user account
    /// based on the retrieved email address. Ensure that the <paramref name="deleteModel"/>
    /// contains a valid staff ID before calling this method.</remarks>
    /// <param name="deleteModel">The model containing the ID of the staff member to delete.</param>
    /// <returns>An <see cref="IActionResult"/> that redirects to the "List" action after the deletion is complete.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(StaffManagementDeleteViewModel deleteModel)
    {
        int id = deleteModel.BasicInfo.Id;
        string userEmail = deleteModel.BasicInfo.EmailAddress;

        if (id <= 0)
            return View("Error", new ErrorViewModel { Message = "Invalid staff ID." });

        // Delete staff record
        var deleteStaffResult = await _apiClient.DeleteAsync($"staff/{id}");
        if (!deleteStaffResult.IsSuccess)
            return View("Error", new ErrorViewModel { Message = deleteStaffResult.ErrorMessage });

        // Delete IdentityUser
        var deleteIdentityUserResult = await _authClient.DeleteUserAsync(userEmail);
        if (!deleteIdentityUserResult.IsSuccess)
            return View("Error", new ErrorViewModel { Message = deleteIdentityUserResult.ErrorMessage });

        TempData["Success"] = $"{deleteModel.BasicInfo.FirstName} {deleteModel.BasicInfo.LastName} profile was successfully deleted.";
        return RedirectToAction("List");
    }
}
