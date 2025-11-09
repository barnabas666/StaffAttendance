using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using StaffAtt.Web.Models;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Helpers;

/// <summary>
/// Service for getting Staff SelectList info.
/// </summary>
public class StaffSelectListService : IStaffSelectListService
{
    private readonly IApiClient _apiClient;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(1); // adjust as needed

    public StaffSelectListService(IApiClient apiClient, IMapper mapper, IMemoryCache cache)
    {
        _apiClient = apiClient;
        _mapper = mapper;
        _cache = cache;
    }

    /// <summary>
    /// Get all Staff from API and create SelectList for dropdown in view.
    /// If defaultValue is provided, add a default item (e.g. "All Staff").
    /// Use caching to reduce API calls.
    /// </summary>
    public async Task<SelectList> GetStaffSelectListAsync(CheckInDisplayAdminViewModel dateDisplayModel,
                                                          string defaultValue = "")
    {
        // Try get cached staff list
        if (!_cache.TryGetValue("staff_basic_list", out List<StaffBasicDto>? staffList))
        {
            var staffResult = await _apiClient.GetAsync<List<StaffBasicDto>>("staff/basic");

            if (!staffResult.IsSuccess || staffResult.Value is null)
            {
                // Instead of crashing UI, return empty dropdown
                return new SelectList(Enumerable.Empty<StaffBasicDto>(), nameof(StaffBasicDto.Id), nameof(StaffBasicDto.FullName));
            }
            staffList = staffResult.Value;

            // Cache result
            _cache.Set("staff_basic_list", staffList, _cacheDuration);
        }

        var listToDisplay = new List<StaffBasicDto>(staffList);

        // Insert default value at the top (e.g. "All Staff")
        if (!string.IsNullOrEmpty(defaultValue))
            listToDisplay.Insert(0, new StaffBasicDto { Id = 0, FirstName = defaultValue });

        // Map and return dropdown
        dateDisplayModel.StaffList = _mapper.Map<List<StaffBasicViewModel>>(staffList);
        // Source is staff, value (Id here) gonna be saved to database, Text (FullName) gets displayed to user.
        return new SelectList(dateDisplayModel.StaffList, nameof(StaffBasicViewModel.Id), nameof(StaffBasicViewModel.FullName));
    }
}
