using StaffAttLibrary.Models;

namespace StaffAtt.Web.Helpers;

public interface IPhoneNumberParser
{
    List<PhoneNumberModel> ParseStringToPhoneNumbers(string phoneNumbersText);
    string ParsePhoneNumbersToString(List<PhoneNumberModel> phoneNumbers);
}