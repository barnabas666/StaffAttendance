
using StaffAttLibrary.Enums;
using StaffAttLibrary.Models;

namespace StaffAttLibrary.Data;

public interface IStaffData
{
    Task<int> CheckAndInsertAliasAsync(string pIN, string alias, int aliasId);
    string CreateAlias(string firstName, string lastName, int orderNumber);
    Task CreatePhoneNumbersAsync(int staffId, List<PhoneNumberModel> phoneNumbers);
    Task DeleteAddressAsync(StaffFullModel staffModel);
    Task DeleteAliasAsync(StaffFullModel staffModel);
    Task DeletePhoneNumbersAsync(int staffId, List<PhoneNumberModel> phoneNumbers);
    Task DeleteStaffAsync(int staffId);
    Task<AddressModel> GetAddressByEmailAsync(string emailAddress);
    Task<AddressModel> GetAddressByIdAsync(int id);
    Task<List<StaffBasicModel>> GetAllBasicStaffByDepartmentAsync(int departmentId);
    Task<List<StaffBasicModel>> GetAllBasicStaffByDepartmentAndApprovedAsync(int departmentId, ApprovedType approvedType);
    Task<List<PhoneNumberModel>> GetPhoneNumbersByStaffIdAsync(int staffId);
    Task<StaffFullModel> GetStaffByEmailAsync(string emailAddress);
    Task<StaffFullModel> GetStaffByIdAsync(int id);
    Task<int> SaveAddressAsync(AddressModel address);
    Task<int> SaveStaffAsync(int departmentId, string firstName, string lastName, string emailAddress, int addressId, int aliasId);
    Task UpdateAddressAsync(AddressModel address, StaffFullModel staff);
    Task UpdateAliasAsync(string pIN, StaffFullModel staff);
    Task UpdateStaffAsync(string firstName, string lastName, StaffFullModel staff);
}
