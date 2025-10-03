using StaffAttShared.DTOs;

namespace StaffAtt.Web.Helpers;

/// <summary>
/// This class is used to parse phone numbers from a string to a list
/// of PhoneNumberDto objects and vice versa.
/// </summary>
public class PhoneNumberDtoParser : IPhoneNumberDtoParser
{
    public List<PhoneNumberDto> ParseStringToPhoneNumbers(string phoneNumbersText)
    {
        return phoneNumbersText
            .Split(',')
            .Select(phone => new PhoneNumberDto { PhoneNumber = phone.Trim() })
            .ToList();
    }

    public string ParsePhoneNumbersToString(List<PhoneNumberDto> phoneNumbers)
    {
        return string.Join(",", phoneNumbers.Select(x => x.PhoneNumber));
    }
}
