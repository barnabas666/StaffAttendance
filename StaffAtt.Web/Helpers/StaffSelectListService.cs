using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    public StaffSelectListService(IApiClient apiClient, IMapper mapper)
    {
        _apiClient = apiClient;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all Staff from API and create SelectList for dropdown in view.
    /// If defaultValue is provided, add a default item (e.g. "All Staff").
    /// </summary>
    public async Task<SelectList> GetStaffSelectListAsync(CheckInDisplayAdminViewModel dateDisplayModel,
                                                          string defaultValue = "")
    {
        // --- Load staff from API ---
        var staffResult = await _apiClient.GetAsync<List<StaffBasicDto>>("staff/basic");

        if (!staffResult.IsSuccess || staffResult.Value is null)
        {
            // Instead of crashing UI, return empty dropdown
            return new SelectList(Enumerable.Empty<StaffBasicDto>(), nameof(StaffBasicDto.Id), nameof(StaffBasicDto.FullName));
        }

        List<StaffBasicDto> staffList = staffResult.Value;

        // Insert default value at the top (e.g. "All Staff")
        if (!string.IsNullOrEmpty(defaultValue))
        {
            staffList.Insert(0, new StaffBasicDto { Id = 0, FirstName = defaultValue });
        }
        dateDisplayModel.StaffList = _mapper.Map<List<StaffBasicViewModel>>(staffList);

        // Source is staff, value (Id here) gonna be saved to database, Text (FullName) gets displayed to user.
        return new SelectList(dateDisplayModel.StaffList, nameof(StaffBasicViewModel.Id), nameof(StaffBasicViewModel.FullName));
    }
}
