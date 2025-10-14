using StaffAttShared.Enums;
using StaffAttLibrary.Models;

namespace StaffAttLibrary.Data;
public interface IStaffService
{
    Task<int> CreateStaffAsync(int departmentId,
                          AddressModel address,
                          string pIN,
                          string firstName,
                          string lastName,
                          string emailAddress,
                          List<PhoneNumberModel> phoneNumbers);
    Task UpdateStaffAsync(AddressModel address,
                          string pIN,
                          string firstName,
                          string lastName,
                          string emailAddress,
                          List<PhoneNumberModel> phoneNumbers);
    Task<AliasModel> AliasVerificationAsync(string alias, string pIN);
    Task<List<DepartmentModel>> GetAllDepartmentsAsync();
    Task<List<StaffBasicModel>> GetAllBasicStaffFilteredAsync(int departmentId, ApprovedType approvedType);
    Task<List<StaffBasicModel>> GetAllBasicStaffAsync();
    Task<StaffFullModel> GetStaffByEmailAsync(string emailAddress);
    Task<StaffFullModel> GetStaffByIdAsync(int id);
    Task<StaffBasicModel> GetBasicStaffByIdAsync(int id);
    Task<StaffBasicModel> GetBasicStaffByAliasIdAsync(int aliasId);
    Task<string> GetStaffEmailByIdAsync(int id);
    Task<bool> CheckStaffByEmailAsync(string emailAddress);
    Task UpdateStaffByAdminAsync(int id, int departmentId, bool isApproved);
    Task DeleteStaffAsync(int staffId);
}
