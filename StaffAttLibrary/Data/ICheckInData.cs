using StaffAttLibrary.Models;

namespace StaffAttLibrary.Data;

public interface ICheckInData
{
    Task<StaffBasicModel> GetBasicStaffByAliasId(int aliasId);
}