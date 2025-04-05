using StaffAttLibrary.Models;

namespace StaffAttLibrary.Data;

public interface ICheckInData
{
    Task<int> CheckInPerform(int staffId);
    Task<int> CheckOutPerform(int checkInId);
    Task<StaffBasicModel> GetBasicStaffByAliasId(int aliasId);
}