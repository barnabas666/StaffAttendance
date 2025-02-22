using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StaffAtt.Web.Models;
using StaffAttLibrary.Data;
using StaffAttLibrary.Enums;
using StaffAttLibrary.Models;

namespace StaffAtt.Web.Controllers;

/// <summary>
/// Controller for Staff Management, Admin use it.
/// </summary>
[Authorize(Roles = "Administrator")]
public class StaffManagementController : Controller
{
    private readonly IDatabaseData _sqlData;
    private readonly IMapper _mapper;

    public StaffManagementController(IDatabaseData sqlData, IMapper mapper)
    {
        this._sqlData = sqlData;
        _mapper = mapper;
    }

    public async Task<IActionResult> List()
    {
        List<StaffBasicModel> staffs = await _sqlData.GetAllBasicStaff();

        StaffManagementListViewModel staffModel = new StaffManagementListViewModel();

        staffModel.BasicInfos = _mapper.Map<List<StaffBasicViewModel>>(staffs);
        
        List<DepartmentModel> departments = await _sqlData.GetAllDepartments();

        // Creating default item = All Departments for DropDown.
        departments.Insert(0, new DepartmentModel()
        {
            Id = 0,
            Title = "All"
        });

        // Source is departments, value (Id here) gonna be saved to database, Text (Title) gets displayed to user.
        staffModel.DepartmentItems = new SelectList(departments,
                                                    nameof(DepartmentModel.Id),
                                                    nameof(DepartmentModel.Title));

        return View(staffModel);
    }

    [HttpPost]
    public async Task<IActionResult> List(StaffManagementListViewModel staffModel)
    {
        ApprovedType approvedType = staffModel.ApprovedRadio;

        List<StaffBasicModel> staffs = await _sqlData.GetAllBasicStaffByDepartmentAndApproved(Convert.ToInt32(staffModel.DepartmentId),
                                                                                              approvedType);

        staffModel.BasicInfos = _mapper.Map<List<StaffBasicViewModel>>(staffs);

        List<DepartmentModel> departments = await _sqlData.GetAllDepartments();

        // Creating default item = All Departments for DropDown.
        departments.Insert(0, new DepartmentModel()
        {
            Id = 0,
            Title = "All"
        });

        // Source is departments, value (Id here) gonna be saved to database, Text (Title) gets displayed to user.
        staffModel.DepartmentItems = new SelectList(departments,
                                                    nameof(DepartmentModel.Id),
                                                    nameof(DepartmentModel.Title));

        return View(staffModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        StaffFullModel staff = await _sqlData.GetStaffById(id);
        StaffDetailsViewModel detailsModel = _mapper.Map<StaffDetailsViewModel>(staff);

        return View(detailsModel);
    }

    public async Task<IActionResult> Update(int id)
    {
        StaffBasicModel basicModel = await _sqlData.GetBasicStaffById(id);

        StaffManagementUpdateViewModel updateModel = _mapper.Map<StaffManagementUpdateViewModel>(basicModel);

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
        StaffBasicModel basicModel = await _sqlData.GetBasicStaffById(id);

        StaffManagementDeleteViewModel deleteModel = _mapper.Map<StaffManagementDeleteViewModel>(basicModel);

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
