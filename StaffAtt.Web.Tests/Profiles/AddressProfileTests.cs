using AutoMapper;
using FluentAssertions;
using StaffAtt.Web.Models;
using StaffAtt.Web.Profiles;
using StaffAttLibrary.Models;

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
