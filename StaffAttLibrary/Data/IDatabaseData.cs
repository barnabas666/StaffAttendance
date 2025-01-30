using StaffAttLibrary.Models;

namespace StaffAttLibrary.Data;
public interface IDatabaseData
{
    Task<AliasModel> AliasVerification(string alias, string pIN);
    Task<bool> CheckApproveStatus(int aliasId);
    Task<int> CheckInPerform(int staffId);
    Task<int> CheckOutPerform(int checkInId);
    void CreateStaff(int departmentId, string street, string city, string zip, string state, string pIN, string firstName, string lastName, string emailAddress, List<string> phoneNumbers);
    Task<List<DepartmentModel>> GetAllDepartments();
    Task<List<StaffFullModel>> GetAllStaff(bool getAll = true);
    Task<CheckInModel> GetLastCheckIn(int staffId);
    Task<StaffFullModel> GetStaffByEmail(string emailAddress);
    Task<bool> CheckStaffByEmail(string emailAddress);
    void UpdateStaffByAdmin(int id, int departmentId, bool isApproved);
}