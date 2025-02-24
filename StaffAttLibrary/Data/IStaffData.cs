using StaffAttLibrary.Enums;
using StaffAttLibrary.Models;

namespace StaffAttLibrary.Data;
public interface IStaffData
{
    Task CreateStaff(int departmentId,
                     AddressModel address,
                     string pIN,
                     string firstName,
                     string lastName,
                     string emailAddress,
                     List<PhoneNumberModel> phoneNumbers);
    Task UpdateStaffProcess(AddressModel address,
                     string pIN,
                     string firstName,
                     string lastName,
                     string emailAddress,
                     List<PhoneNumberModel> phoneNumbers);
    Task<AliasModel> AliasVerification(string alias, string pIN);
    Task<List<DepartmentModel>> GetAllDepartments();
    Task<List<StaffBasicModel>> GetAllBasicStaffFiltered(int departmentId, ApprovedType approvedType);
    Task<List<StaffBasicModel>> GetAllBasicStaff();
    Task<StaffFullModel> GetStaffByEmailProcess(string emailAddress);
    Task<StaffFullModel> GetStaffByIdProcess(int id);
    Task<StaffBasicModel> GetBasicStaffById(int id);
    Task<StaffBasicModel> GetBasicStaffByAliasId(int aliasId);
    Task<bool> CheckStaffByEmail(string emailAddress);
    Task UpdateStaffByAdmin(int id, int departmentId, bool isApproved);
    Task DeleteStaffProcess(int staffId);
}