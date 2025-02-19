using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        List<StaffManagementListViewModel> staffsList = new List<StaffManagementListViewModel>();
        List<StaffBasicModel> staffs = await _sqlData.GetAllBasicStaff();

        foreach (StaffBasicModel staff in staffs)
        {
            StaffManagementListViewModel viewModel = new StaffManagementListViewModel();
            viewModel.BasicInfo.Id = staff.Id;
            viewModel.BasicInfo.FirstName = staff.FirstName;
            viewModel.BasicInfo.LastName = staff.LastName;
            viewModel.BasicInfo.EmailAddress = staff.EmailAddress;
            viewModel.BasicInfo.Alias = staff.Alias;
            viewModel.BasicInfo.Title = staff.Title;
            viewModel.BasicInfo.IsApproved = staff.IsApproved;

            staffsList.Add(viewModel);
        }

        return View(staffsList);
    }

    public async Task<IActionResult> Update(int id)
    {
        StaffManagementUpdateViewModel updateModel = new StaffManagementUpdateViewModel();
        StaffBasicModel basicModel = await _sqlData.GetBasicStaffById(id);

        updateModel.BasicInfo.Id = basicModel.Id;
        updateModel.BasicInfo.FirstName = basicModel.FirstName;
        updateModel.BasicInfo.LastName = basicModel.LastName;
        updateModel.BasicInfo.EmailAddress = basicModel.EmailAddress;
        updateModel.BasicInfo.Alias = basicModel.Alias;
        updateModel.BasicInfo.IsApproved = basicModel.IsApproved;
        updateModel.BasicInfo.DepartmentId = basicModel.DepartmentId;
        updateModel.BasicInfo.Title = basicModel.Title;

        // We get all Departments from our database.
        List<DepartmentModel> departments = await _sqlData.GetAllDepartments();

        // Source is departments, value (Id here) gonna be saved to database,
        // Text (Title) gets displayed to user, both expect string.
        updateModel.DepartmentItems = new SelectList(departments,
                                                     nameof(DepartmentModel.Id),
                                                     nameof(DepartmentModel.Title));

        return View(updateModel);
    }

    [HttpPost]
    public async Task<IActionResult> Update(StaffManagementUpdateViewModel updateModel)
    {
        await _sqlData.UpdateStaffByAdmin(updateModel.BasicInfo.Id,
                                          Convert.ToInt32(updateModel.BasicInfo.DepartmentId),
                                          updateModel.BasicInfo.IsApproved);

        return RedirectToAction("List");
    }

    /// <summary>
    /// Get Action call. We get StaffBasicModel by Id (passed in URL from List Page after we hit Delete button).
    /// </summary>
    /// <param name="id">Staff's id.</param>
    /// <returns>View with populated StaffBasicModel to delete inside.</returns>
    public async Task<IActionResult> Delete(int id)
    {
        StaffManagementDeleteViewModel deleteModel = new StaffManagementDeleteViewModel();
        StaffBasicModel basicModel = await _sqlData.GetBasicStaffById(id);

        deleteModel.BasicInfo.Id = basicModel.Id;
        deleteModel.BasicInfo.FirstName = basicModel.FirstName;
        deleteModel.BasicInfo.LastName = basicModel.LastName;
        deleteModel.BasicInfo.EmailAddress = basicModel.EmailAddress;
        deleteModel.BasicInfo.Alias = basicModel.Alias;
        deleteModel.BasicInfo.Title = basicModel.Title;

        // return View with our Staff Info to delete.
        return View(deleteModel);
    }

    /// <summary>
    /// Post Action call. After hit Submit button we delete Staff from our database.
    /// </summary>
    /// <param name="staffModel">Staff to delete info.</param>
    /// <returns>Redirect back to List Action.</returns>
    [HttpPost]
    public async Task<IActionResult> Delete(StaffBasicModel staffModel)
    {
        await _sqlData.DeleteStaff(staffModel.Id);

        // After deleting Staff we redirect back to List Action.
        return RedirectToAction("List");
    }
}
