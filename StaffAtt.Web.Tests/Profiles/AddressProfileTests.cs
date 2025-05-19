using AutoMapper;
using StaffAtt.Web.Models;
using StaffAttLibrary.Models;
using FluentAssertions;
using StaffAtt.Web.Profiles;

namespace StaffAtt.Web.Tests.Profiles;
public class AddressProfileTests
{
    [Fact]
    public void TestAddressMapping()
    {
        // Arrange
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AddressProfile>());
        var mapper = config.CreateMapper();
        var addressModel = new AddressModel
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
