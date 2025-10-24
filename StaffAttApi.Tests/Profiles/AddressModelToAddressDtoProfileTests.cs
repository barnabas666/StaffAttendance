using AutoMapper;
using FluentAssertions;
using StaffAttApi.Profiles;
using StaffAttLibrary.Models;
using StaffAttShared.DTOs;

namespace StaffAttApi.Tests.Profiles;
public class AddressModelToAddressDtoProfileTests
{
    [Fact]
    public void TestAddressMapping()
    {
        // Arrange
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AddressModelToAddressDtoProfile>());
        var mapper = config.CreateMapper();

        var addressModel = new AddressModel
        {
            Id = 10,
            Street = "Main St 123",
            City = "Springfield",
            Zip = "98765",
            State = "IL"
        };

        // Act
        var addressDto = mapper.Map<AddressDto>(addressModel);

        // Assert
        addressDto.Should().BeEquivalentTo(addressModel, options => options.ExcludingMissingMembers());
    }
}
