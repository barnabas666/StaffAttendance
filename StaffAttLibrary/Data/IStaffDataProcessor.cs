using StaffAttLibrary.Models;

namespace StaffAttLibrary.Data;
public interface IStaffDataProcessor
{
    Task<int> CreatePhoneNumberAsync(int phoneNumberId, PhoneNumberModel phoneNumber);
    Task CreatePhoneNumberLinkAsync(int staffId, int phoneNumberId);
    Task DeletePhoneNumberAsync(int phoneNumberId);
    Task DeletePhoneNumberLinkAsync(int staffId, int phoneNumberId);
    Task<List<PhoneNumberModel>> GetByPhoneNumberAsync(PhoneNumberModel phoneNumber);
    Task<List<StaffPhoneNumberModel>> GetPhoneNumberLinksAsync(int phoneNumberId);
}
