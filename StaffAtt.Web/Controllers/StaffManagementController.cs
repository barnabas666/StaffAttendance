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
        List<StaffBasicModel> staffs = await _sqlData.GetAllBasicStaff();

        return View(staffs);
    }

    public async Task<IActionResult> Update(int id)
    {
        StaffManagementUpdateModel updateModel = new StaffManagementUpdateModel();

        updateModel.BasicInfo = await _sqlData.GetBasicStaffById(id);

        // We get all Departments from our database.
        List<DepartmentModel> departments = await _sqlData.GetAllDepartments();

        // Source is departments, value (Id here) gonna be saved to database, Text (Title) gets displayed to user, both expect string.
        updateModel.DepartmentItems = new SelectList(departments, nameof(DepartmentModel.Id), nameof(DepartmentModel.Title));        

        return View(updateModel);
    }

    [HttpPost]
    public async Task<IActionResult> Update(StaffManagementUpdateModel updateModel)
    {
        await _sqlData.UpdateStaffByAdmin(updateModel.BasicInfo.Id, Convert.ToInt32(updateModel.BasicInfo.DepartmentId), updateModel.BasicInfo.IsApproved);

        return RedirectToAction("List");
    }
}
