
using StaffAttLibrary.Enums;
using StaffAttLibrary.Models;

namespace StaffAttLibrary.Data;

public interface IStaffData
{
    Task<int> CheckAndInsertAlias(string pIN, string alias, int aliasId);
    string CreateAlias(string firstName, string lastName, int orderNumber);
    Task CreatePhoneNumbers(int staffId, List<PhoneNumberModel> phoneNumbers);
    Task DeleteAddress(StaffFullModel staffModel);
    Task DeleteAlias(StaffFullModel staffModel);
    Task DeletePhoneNumbers(int staffId, List<PhoneNumberModel> phoneNumbers);
    Task DeleteStaff(int staffId);
    Task<AddressModel> GetAddressByEmail(string emailAddress);
    Task<AddressModel> GetAddressById(int id);
    Task<List<StaffBasicModel>> GetAllBasicStaffByDepartment(int departmentId);
    Task<List<StaffBasicModel>> GetAllBasicStaffByDepartmentAndApproved(int departmentId, ApprovedType approvedType);
    Task<List<PhoneNumberModel>> GetPhoneNumbersByStaffId(int staffId);
    Task<StaffFullModel> GetStaffByEmail(string emailAddress);
    Task<StaffFullModel> GetStaffById(int id);
    Task<int> SaveAddress(AddressModel address);
    Task<int> SaveStaff(int departmentId, string firstName, string lastName, string emailAddress, int addressId, int aliasId);
    Task UpdateAddress(AddressModel address, StaffFullModel staff);
    Task UpdateAlias(string pIN, StaffFullModel staff);
    Task UpdateStaff(string firstName, string lastName, StaffFullModel staff);
}