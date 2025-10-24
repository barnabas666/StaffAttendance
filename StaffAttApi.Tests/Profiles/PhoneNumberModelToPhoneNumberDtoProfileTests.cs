using AutoMapper;
using FluentAssertions;
using StaffAttApi.Profiles;
using StaffAttLibrary.Models;
using StaffAttShared.DTOs;

namespace StaffAttApi.Tests.Profiles;
public class PhoneNumberModelToPhoneNumberDtoProfileTests
{
    [Fact]
    public void TestPhoneNumberMapping()
    {
        // Arrange
        var config = new MapperConfiguration(cfg => cfg.AddProfile<PhoneNumberModelToPhoneNumberDtoProfile>());
        var mapper = config.CreateMapper();

        var phoneNumberModel = new PhoneNumberModel
        {
            Id = 7,
            PhoneNumber = "+420777888999"
        };

        // Act
        var phoneNumberDto = mapper.Map<PhoneNumberDto>(phoneNumberModel);

        // Assert — compare properties manually to avoid Equals() interference
        phoneNumberDto.Id.Should().Be(phoneNumberModel.Id);
        phoneNumberDto.PhoneNumber.Should().Be(phoneNumberModel.PhoneNumber);
    }
}
