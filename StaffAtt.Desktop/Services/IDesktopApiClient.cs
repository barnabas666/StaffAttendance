using StaffAtt.Desktop.Models;
using StaffAttShared.DTOs;

namespace StaffAtt.Desktop.Helpers;

public interface IDesktopApiClient
{
    Task<DesktopResult<DesktopLoginResponse>> LoginAsync(string alias, string pin);
    Task<DesktopResult<StaffBasicDto>> GetStaffByAliasIdAsync(int aliasId);
    Task<DesktopResult<CheckInDto?>> GetLastCheckInAsync(int staffId);
    Task<DesktopResult<bool>> DoCheckInAsync(int staffId);
}
