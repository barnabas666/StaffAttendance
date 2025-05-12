using StaffAttLibrary.Models;

namespace StaffAttLibrary.Data;
public interface IStaffDataProcessor
{
    Task<int> SavePhoneNumberAsync(PhoneNumberModel phoneNumber);
    Task SavePhoneNumberLinkAsync(int staffId, int phoneNumberId);
    Task DeletePhoneNumberAsync(int phoneNumberId);
    Task DeletePhoneNumberLinkAsync(int staffId, int phoneNumberId);
    Task<bool> CheckPhoneNumberAsync(PhoneNumberModel phoneNumber);
    Task<List<PhoneNumberModel>> GetPhoneNumberAsync(PhoneNumberModel phoneNumber);
    Task<List<StaffPhoneNumberModel>> GetPhoneNumberLinksAsync(int phoneNumberId);
    Task<bool> CheckAliasAsync(string alias);
    Task<int> SaveAliasAsync(string alias, string pIN);
    string CreateAlias(string firstName, string lastName, int orderNumber);
}
