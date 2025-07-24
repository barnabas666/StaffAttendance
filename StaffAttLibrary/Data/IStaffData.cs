
using StaffAttLibrary.Enums;
using StaffAttLibrary.Models;

namespace StaffAttLibrary.Data;

public interface IStaffData
{
    Task<int> CreateAliasAsync(string pIN, string firstName, string lastName);
    Task CreatePhoneNumbersAsync(int staffId, List<PhoneNumberModel> phoneNumbers);
    Task DeleteAddressAsync(int addressId);
    Task DeleteAliasAsync(int aliasId);
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
    Task UpdateAddressAsync(AddressModel address, int addressId);
    Task UpdateAliasAsync(string pIN, int aliasId);
    Task UpdateStaffAsync(string firstName, string lastName, int staffId);
}
