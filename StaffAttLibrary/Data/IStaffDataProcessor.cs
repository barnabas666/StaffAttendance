using StaffAttLibrary.Models;

namespace StaffAttLibrary.Data;
public interface IStaffDataProcessor
{
    Task<int> CreatePhoneNumber(int phoneNumberId, PhoneNumberModel phoneNumber);
    Task CreatePhoneNumberLink(int staffId, int phoneNumberId);
    Task DeletePhoneNumber(int phoneNumberId);
    Task DeletePhoneNumberLink(int staffId, int phoneNumberId);
    Task<List<PhoneNumberModel>> GetByPhoneNumber(PhoneNumberModel phoneNumber);
    Task<List<StaffPhoneNumberModel>> GetPhoneNumberLinks(int phoneNumberId);
}