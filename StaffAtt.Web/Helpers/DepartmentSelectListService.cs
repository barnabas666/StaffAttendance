using Microsoft.AspNetCore.Mvc.Rendering;
using StaffAttLibrary.Data;
using StaffAttLibrary.Models;

namespace StaffAtt.Web.Helpers;

/// <summary>
/// Service for getting Departments SelectList info.
/// </summary>
public class DepartmentSelectListService : IDepartmentSelectListService
{
    private readonly IStaffService _staffService;

    public DepartmentSelectListService(IStaffService staffService)
    {
        _staffService = staffService;
    }

    /// <summary>
    /// Get all Departments from database and create SelectList for DropDown in View.
    /// If defaultValue is not empty, we create default item = All Departments for DropDown.
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public async Task<SelectList> GetDepartmentSelectListAsync(string defaultValue = "")
    {
        // We get all Departments from our database.
        var departments = await _staffService.GetAllDepartmentsAsync();

        if (string.IsNullOrEmpty(defaultValue) == false)
        {
            // Creating default item = All Departments for DropDown.
            departments.Insert(0, new DepartmentModel()
            {
                Id = 0,
                Title = defaultValue
            });
        }

        // Source is departments, value (Id here) gonna be saved to database, Text (Title) gets displayed to user, both expect string.
        return new SelectList(departments, nameof(DepartmentModel.Id), nameof(DepartmentModel.Title));
    }
}
