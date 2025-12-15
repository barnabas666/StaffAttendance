using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Services;

/// <summary>
/// Service for getting Departments SelectList info.
/// </summary>
public class DepartmentSelectListService : IDepartmentSelectListService
{
    private readonly IApiClient _apiClient;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(1);

    public DepartmentSelectListService(IApiClient apiClient, IMemoryCache cache)
    {
        _apiClient = apiClient;
        _cache = cache;
    }

    /// <summary>
    /// Get all Departments from database and create SelectList for DropDown in View.
    /// If defaultValue is not empty, we create default item = All Departments for DropDown.
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public async Task<SelectList> GetDepartmentSelectListAsync(string defaultValue = "")
    {
        // Try read from cache
        if (!_cache.TryGetValue("department_list", out List<DepartmentDto>? departments))
        {
            // Not cached => call API
            var result = await _apiClient.GetAsync<List<DepartmentDto>>("staff/departments");

            // Instead of crashing UI, return empty dropdown
            if (!result.IsSuccess || result.Value is null)           
                return new SelectList(Enumerable.Empty<DepartmentDto>(), nameof(DepartmentDto.Id), nameof(DepartmentDto.Title));
            
            departments = result.Value;

            // Cache result
            _cache.Set("department_list", departments, _cacheDuration);
        }

        // Copy list so we don’t mutate the cached version (we insert default value below which we don’t want cached)
        var listToDisplay = new List<DepartmentDto>(departments);

        // Insert default value at the top (e.g. "All Departments")
        if (!string.IsNullOrEmpty(defaultValue))  
            listToDisplay.Insert(0, new DepartmentDto { Id = 0, Title = defaultValue });        

        // Source is departments, value (Id here) gonna be saved to database, Text (Title) gets displayed to user, both expect string.
        return new SelectList(listToDisplay, nameof(DepartmentDto.Id), nameof(DepartmentDto.Title));
    }
}
