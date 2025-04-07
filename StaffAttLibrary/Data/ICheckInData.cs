using StaffAttLibrary.Models;

namespace StaffAttLibrary.Data;

public interface ICheckInData
{
    Task<int> CheckInPerformAsync(int staffId);
    Task<int> CheckOutPerformAsync(int checkInId);
    Task<StaffBasicModel> GetBasicStaffByAliasIdAsync(int aliasId);
}
