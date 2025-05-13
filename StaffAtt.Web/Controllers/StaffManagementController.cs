using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StaffAtt.Web.Helpers;
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
    private readonly IStaffService _staffService;
    private readonly IMapper _mapper;
    private readonly IDepartmentSelectListService _departmentService;

    public StaffManagementController(IStaffService staffService, IMapper mapper, IDepartmentSelectListService departmentService)
    {
        this._staffService = staffService;
        _mapper = mapper;
        _departmentService = departmentService;
    }

    public async Task<IActionResult> List()
    {
        StaffManagementListViewModel staffModel = new StaffManagementListViewModel();
        List<StaffBasicModel> staffs = await _staffService.GetAllBasicStaffAsync();
        staffModel.BasicInfos = _mapper.Map<List<StaffBasicViewModel>>(staffs);
        
        staffModel.DepartmentItems = await _departmentService.GetDepartmentSelectListAsync("All");

        return View(staffModel);
    }

    [HttpPost]
    public async Task<IActionResult> List(StaffManagementListViewModel staffModel)
    {
        List<StaffBasicModel> staffs = await _staffService.GetAllBasicStaffFilteredAsync(Convert.ToInt32(staffModel.DepartmentId),
                                                                                              staffModel.ApprovedRadio);
        staffModel.BasicInfos = _mapper.Map<List<StaffBasicViewModel>>(staffs);

        staffModel.DepartmentItems = await _departmentService.GetDepartmentSelectListAsync("All");

        return View(staffModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        StaffFullModel staff = await _staffService.GetStaffByIdAsync(id);
        StaffDetailsViewModel detailsModel = _mapper.Map<StaffDetailsViewModel>(staff);

        return View(detailsModel);
    }

    public async Task<IActionResult> Update(int id)
    {
        StaffBasicModel basicModel = await _staffService.GetBasicStaffByIdAsync(id);
        StaffManagementUpdateViewModel updateModel = _mapper.Map<StaffManagementUpdateViewModel>(basicModel);

        updateModel.DepartmentItems = await _departmentService.GetDepartmentSelectListAsync(String.Empty);

        return View(updateModel);
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
    /// Get Action call. We get StaffBasicModel by Id (passed in URL from List Page after we hit Delete button).
    /// </summary>
    /// <param name="id">Staff's id.</param>
    /// <returns>View with populated StaffBasicModel to delete inside.</returns>
    public async Task<IActionResult> Delete(int id)
    {
        StaffBasicModel basicModel = await _staffService.GetBasicStaffByIdAsync(id);

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
        await _staffService.DeleteStaffAsync(staffModel.Id);

        // After deleting Staff we redirect back to List Action.
        return RedirectToAction("List");
    }
}
