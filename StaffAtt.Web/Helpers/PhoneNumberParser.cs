using StaffAttLibrary.Models;

namespace StaffAtt.Web.Helpers;

/// <summary>
/// This class is used to parse phone numbers from a string to a list
/// of PhoneNumberModel objects and vice versa.
/// </summary>
public class PhoneNumberParser : IPhoneNumberParser
{
    public List<PhoneNumberModel> ParseStringToPhoneNumbers(string phoneNumbersText)
    {
        return phoneNumbersText
            .Split(',')
            .Select(phone => new PhoneNumberModel { PhoneNumber = phone.Trim() })
            .ToList();
    }

    public string ParsePhoneNumbersToString(List<PhoneNumberModel> phoneNumbers)
    {
        return string.Join(",", phoneNumbers.Select(x => x.PhoneNumber));
    }
}
