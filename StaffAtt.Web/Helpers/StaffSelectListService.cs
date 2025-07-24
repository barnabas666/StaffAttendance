using Microsoft.AspNetCore.Mvc.Rendering;
using StaffAtt.Web.Models;
using StaffAttLibrary.Data;
using StaffAttLibrary.Models;

namespace StaffAtt.Web.Helpers;

/// <summary>
/// Service for getting Staff SelectList info.
/// </summary>
public class StaffSelectListService : IStaffSelectListService
{
    private readonly IStaffService _staffService;

    public StaffSelectListService(IStaffService staffService)
    {
        _staffService = staffService;
    }

    /// <summary>
    /// Get all Staff from database and create SelectList for DropDown in View.
    /// If defaultValue is not empty, we create default item = All Staff for DropDown.
    /// </summary>
    /// <param name="dateDisplayModel"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public async Task<SelectList> GetStaffSelectListAsync(CheckInDisplayAdminViewModel dateDisplayModel,
                                                          string defaultValue = "")
    {
        if (string.IsNullOrEmpty(defaultValue) == false)
        {
            // Creating default item = All Staff for DropDown.
            dateDisplayModel.StaffList.Insert(0, new StaffBasicViewModel()
            {
                Id = 0,
                FirstName = defaultValue
            });
        }

        // Source is staff, value (Id here) gonna be saved to database, Text (FullName) gets displayed to user.
        return new SelectList(dateDisplayModel.StaffList, nameof(StaffBasicModel.Id), nameof(StaffBasicModel.FullName));
    }
}
