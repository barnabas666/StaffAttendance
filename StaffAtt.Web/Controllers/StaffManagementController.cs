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
    private readonly IStaffData _staffData;
    private readonly IMapper _mapper;

    public StaffManagementController(IStaffData staffData, IMapper mapper)
    {
        this._staffData = staffData;
        _mapper = mapper;
    }

    public async Task<IActionResult> List()
    {
        StaffManagementListViewModel staffModel = new StaffManagementListViewModel();
        List<StaffBasicModel> staffs = await _staffData.GetAllBasicStaff();
        staffModel.BasicInfos = _mapper.Map<List<StaffBasicViewModel>>(staffs);
        
        List<DepartmentModel> departments = await _staffData.GetAllDepartments();
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
        List<StaffBasicModel> staffs = await _staffData.GetAllBasicStaffFiltered(Convert.ToInt32(staffModel.DepartmentId),
                                                                                              staffModel.ApprovedRadio);
        staffModel.BasicInfos = _mapper.Map<List<StaffBasicViewModel>>(staffs);

        List<DepartmentModel> departments = await _staffData.GetAllDepartments();
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
        StaffFullModel staff = await _staffData.GetStaffByIdProcess(id);
        StaffDetailsViewModel detailsModel = _mapper.Map<StaffDetailsViewModel>(staff);

        return View(detailsModel);
    }

    public async Task<IActionResult> Update(int id)
    {
        StaffBasicModel basicModel = await _staffData.GetBasicStaffById(id);
        StaffManagementUpdateViewModel updateModel = _mapper.Map<StaffManagementUpdateViewModel>(basicModel);

        // We get all Departments from our database.
        List<DepartmentModel> departments = await _staffData.GetAllDepartments();
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
        await _staffData.UpdateStaffByAdmin(updateModel.BasicInfo.Id,
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
        StaffBasicModel basicModel = await _staffData.GetBasicStaffById(id);

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
        await _staffData.DeleteStaffProcess(staffModel.Id);

        // After deleting Staff we redirect back to List Action.
        return RedirectToAction("List");
    }
}
