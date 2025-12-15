using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using StaffAtt.Web.Models;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Services;

/// <summary>
/// Service for getting Staff SelectList info.
/// </summary>
public class StaffSelectListService : IStaffSelectListService
{
    private readonly IApiClient _apiClient;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(1);

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
            // Not cached => call API
            var staffResult = await _apiClient.GetAsync<List<StaffBasicDto>>("staff/basic");

            // Instead of crashing UI, return empty dropdown
            if (!staffResult.IsSuccess || staffResult.Value is null)            
                return new SelectList(Enumerable.Empty<StaffBasicDto>(), nameof(StaffBasicDto.Id), nameof(StaffBasicDto.FullName));
            
            staffList = staffResult.Value;

            // Cache result
            _cache.Set("staff_basic_list", staffList, _cacheDuration);
        }

        // Copy list so we don’t mutate the cached version (we insert default value below which we don’t want cached)
        var listToDisplay = new List<StaffBasicDto>(staffList);

        // Insert default value at the top (e.g. "All Staff")
        if (!string.IsNullOrEmpty(defaultValue))
            listToDisplay.Insert(0, new StaffBasicDto { Id = 0, FirstName = defaultValue });

        // Map and return dropdown
        dateDisplayModel.StaffList = _mapper.Map<List<StaffBasicViewModel>>(listToDisplay);
        // Source is staff, value (Id here) gonna be saved to database, Text (FullName) gets displayed to user.
        return new SelectList(dateDisplayModel.StaffList, nameof(StaffBasicViewModel.Id), nameof(StaffBasicViewModel.FullName));
    }
}
