using StaffAttLibrary.Models;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Helpers;

public interface IPhoneNumberParser
{
    List<PhoneNumberDto> ParseStringToPhoneNumbers(string phoneNumbersText);
    string ParsePhoneNumbersToString(List<PhoneNumberDto> phoneNumbers);
}