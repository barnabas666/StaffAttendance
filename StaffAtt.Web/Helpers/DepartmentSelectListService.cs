using Microsoft.AspNetCore.Mvc.Rendering;
using StaffAttLibrary.Data;
using StaffAttLibrary.Models;

namespace StaffAtt.Web.Helpers;

/// <summary>
/// Service for getting Departments SelectList info.
/// </summary>
public class DepartmentSelectListService : IDepartmentSelectListService
{
    private readonly IApiClient _apiClient;

    public DepartmentSelectListService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    /// <summary>
    /// Get all Departments from database and create SelectList for DropDown in View.
    /// If defaultValue is not empty, we create default item = All Departments for DropDown.
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public async Task<SelectList> GetDepartmentSelectListAsync(string defaultValue = "")
    {
        // Call API
        var result = await _apiClient.GetAsync<List<DepartmentModel>>("staff/departments");

        if (!result.IsSuccess || result.Value is null)
        {
            // Instead of crashing UI, return empty dropdown
            return new SelectList(Enumerable.Empty<DepartmentModel>(), nameof(DepartmentModel.Id), nameof(DepartmentModel.Title));
        }

        var departments = result.Value;

        if (!string.IsNullOrEmpty(defaultValue))
        {
            // Creating default item = All Departments for DropDown.
            departments.Insert(0, new DepartmentModel { Id = 0, Title = defaultValue });
        }

        // Source is departments, value (Id here) gonna be saved to database, Text (Title) gets displayed to user, both expect string.
        return new SelectList(departments, nameof(DepartmentModel.Id), nameof(DepartmentModel.Title));
    }
}
