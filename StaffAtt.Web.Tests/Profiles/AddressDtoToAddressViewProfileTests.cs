using AutoMapper;
using FluentAssertions;
using StaffAtt.Web.Models;
using StaffAtt.Web.Profiles;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Tests.Profiles;
public class AddressDtoToAddressViewProfileTests
{
    [Fact]
    public void TestAddressMapping()
    {
        // Arrange
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AddressDtoToAddressViewProfile>());
        var mapper = config.CreateMapper();
        var addressModel = new AddressDto
        {
            Street = "123 Main St",
            City = "Anytown",
            Zip = "12345",
            State = "CA"
        };
        // Act
        var addressViewModel = mapper.Map<AddressViewModel>(addressModel);
        // Assert
        addressViewModel.Should().BeEquivalentTo(addressModel, options => options.ExcludingMissingMembers());
    }
}
