using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StaffAtt.Web.Helpers;
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
    private readonly IStaffService _staffService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IDepartmentSelectListService _departmentService;

    public StaffManagementController(IStaffService staffService,
                                     IUserService userService,
                                     IMapper mapper,
                                     IDepartmentSelectListService departmentService)
    {
        _staffService = staffService;
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
    public async Task<IActionResult> List()
    {
        StaffManagementListViewModel staffModel = new StaffManagementListViewModel();
        List<StaffBasicModel> staffs = await _staffService.GetAllBasicStaffAsync();
        staffModel.BasicInfos = _mapper.Map<List<StaffBasicViewModel>>(staffs);

        staffModel.DepartmentItems = await _departmentService.GetDepartmentSelectListAsync("All");

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
        List<StaffBasicModel> staffs = await _staffService.GetAllBasicStaffFilteredAsync(Convert.ToInt32(staffModel.DepartmentId),
                                                                                              staffModel.ApprovedRadio);
        staffModel.BasicInfos = _mapper.Map<List<StaffBasicViewModel>>(staffs);

        staffModel.DepartmentItems = await _departmentService.GetDepartmentSelectListAsync("All");

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
    public async Task<IActionResult> Details(int id)
    {
        StaffFullModel staff = await _staffService.GetStaffByIdAsync(id);
        StaffDetailsViewModel detailsModel = _mapper.Map<StaffDetailsViewModel>(staff);

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
    public async Task<IActionResult> Update(int id)
    {
        StaffBasicModel basicModel = await _staffService.GetBasicStaffByIdAsync(id);
        StaffManagementUpdateViewModel updateModel = _mapper.Map<StaffManagementUpdateViewModel>(basicModel);

        updateModel.DepartmentItems = await _departmentService.GetDepartmentSelectListAsync(String.Empty);

        return View("Update", updateModel);
    }

    [HttpPost]
    public async Task<IActionResult> Update(StaffManagementUpdateViewModel updateModel)
    {
        await _staffService.UpdateStaffByAdminAsync(updateModel.BasicInfo.Id,
                                          Convert.ToInt32(updateModel.BasicInfo.DepartmentId),
                                          updateModel.BasicInfo.IsApproved);

        return RedirectToAction("List");
    }

    /// <summary>
    /// Displays the delete confirmation view for a staff member with the specified ID.
    /// </summary>
    /// <remarks>We get StaffBasicModel by Id (passed in URL from List Page after we hit Delete button)<\remarks>
    /// <param name="id">The unique identifier of the staff member to delete.</param>
    /// <returns>An <see cref="IActionResult"/> that renders the delete confirmation view populated with the staff member's
    /// details.</returns>
    public async Task<IActionResult> Delete(int id)
    {
        StaffBasicModel basicModel = await _staffService.GetBasicStaffByIdAsync(id);

        StaffManagementDeleteViewModel deleteModel = _mapper.Map<StaffManagementDeleteViewModel>(basicModel);

        // return View with our Staff Info to delete.
        return View("Delete", deleteModel);
    }

    /// <summary>
    /// Deletes a staff member and their associated user account.
    /// </summary>
    /// <remarks>This method performs the following actions: Retrieves
    /// the email address of the staff member using their ID. Deletes the staff
    /// member from the system. Finds and deletes the associated user account
    /// based on the retrieved email address. Ensure that the <paramref name="staffModel"/>
    /// contains a valid staff ID before calling this method.</remarks>
    /// <param name="staffModel">The model containing the ID of the staff member to delete.</param>
    /// <returns>An <see cref="IActionResult"/> that redirects to the "List" action after the deletion is complete.</returns>
    [HttpPost]
    public async Task<IActionResult> Delete(StaffBasicModel staffModel)
    {
        string userEmail = await _staffService.GetStaffEmailByIdAsync(staffModel.Id);
        await _staffService.DeleteStaffAsync(staffModel.Id);

        IdentityUser userToDelete = await _userService.FindByEmailAsync(userEmail);
        await _userService.DeleteIdentityUserAsync(userToDelete);

        // After deleting Staff we redirect back to List Action.
        return RedirectToAction("List");
    }
}
