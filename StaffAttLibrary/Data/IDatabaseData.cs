using StaffAttLibrary.Models;

namespace StaffAttLibrary.Data;
public interface IDatabaseData
{
    Task<AliasModel> AliasVerification(string alias, string pIN);
    Task<bool> CheckApproveStatus(int aliasId);
    Task<int> CheckInPerform(int staffId);
    Task<int> CheckOutPerform(int checkInId);
    Task CreateStaff(int departmentId, string street, string city, string zip, string state, string pIN, string firstName, string lastName, string emailAddress, List<string> phoneNumbers);
    Task<List<DepartmentModel>> GetAllDepartments();
    Task<List<StaffFullModel>> GetAllStaff(bool getAll = true);
    Task<List<StaffBasicModel>> GetAllBasicStaff();
    Task<CheckInModel> GetLastCheckIn(int staffId);
    Task<StaffFullModel> GetStaffByEmail(string emailAddress);
    Task<StaffFullModel> GetStaffById(int id);
    Task<StaffBasicModel> GetBasicStaffById(int id);
    Task<StaffBasicModel> GetBasicStaffByAliasId(int aliasId);
    Task<bool> CheckStaffByEmail(string emailAddress);
    Task UpdateStaffByAdmin(int id, int departmentId, bool isApproved);
    Task DeleteStaff(int staffId);
    Task<List<CheckInFullModel>> GetAllCheckIns();
    Task<List<CheckInFullModel>> GetCheckInsByEmail(string emailAddress);
}