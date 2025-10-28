using StaffAttShared.DTOs;

namespace StaffAtt.Web.Helpers;

public interface IPhoneNumberDtoParser
{
    List<PhoneNumberDto> ParseStringToPhoneNumbers(string phoneNumbersText);
    string ParsePhoneNumbersToString(List<PhoneNumberDto> phoneNumbers);
}