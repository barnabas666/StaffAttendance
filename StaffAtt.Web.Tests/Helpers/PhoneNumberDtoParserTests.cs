using FluentAssertions;
using StaffAtt.Web.Helpers;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Tests.Helpers;
public class PhoneNumberDtoParserTests
{
    private readonly PhoneNumberDtoParser _sut;

    public PhoneNumberDtoParserTests()
    {
        _sut = new PhoneNumberDtoParser();
    }

    [Fact]
    public void ParseStringToPhoneNumbers_ShouldReturnPhoneNumbers()
    {
        // Arrange
        string input = "111222333,444555666,777888999";
        var expected = new List<PhoneNumberDto>
        {
            new PhoneNumberDto { PhoneNumber = "111222333" },
            new PhoneNumberDto { PhoneNumber = "444555666" },
            new PhoneNumberDto { PhoneNumber = "777888999" }
        };
        // Act
        var result = _sut.ParseStringToPhoneNumbers(input);
        // Assert
        result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [Fact]
    public void ParsePhoneNumbersToString_ShouldReturnString()
    {
        // Arrange
        var phoneNumbers = new List<PhoneNumberDto>
        {
            new PhoneNumberDto { PhoneNumber = "111222333" },
            new PhoneNumberDto { PhoneNumber = "444555666" },
            new PhoneNumberDto { PhoneNumber = "777888999" }
        };
        string expected = "111222333,444555666,777888999";
        // Act
        var result = _sut.ParsePhoneNumbersToString(phoneNumbers);
        // Assert
        result.Should().Be(expected);
    }
}
