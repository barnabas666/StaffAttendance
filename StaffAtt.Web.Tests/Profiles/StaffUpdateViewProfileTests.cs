using AutoMapper;
using FluentAssertions;
using StaffAtt.Web.Models;
using StaffAtt.Web.Profiles;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Tests.Profiles;
public class StaffUpdateViewProfileTests
{
    [Fact]
    public void TestStaffUpdateViewMapping()
    {
        // Arrange
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<StaffFullDtoToStaffUpdateViewProfile>();
            cfg.AddProfile<AddressDtoToAddressViewProfile>();
        });
        var mapper = config.CreateMapper();
        var staffFullModel = new StaffFullDto
        {
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john.doe@johndoe.com",
            Address = new AddressDto
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "CA",
                Zip = "12345"
            }
        };
        // Act
        var staffUpdateViewModel = mapper.Map<StaffUpdateViewModel>(staffFullModel);
        // Assert
        staffUpdateViewModel.Should().BeEquivalentTo(staffFullModel,
                                                     options => options.ExcludingMissingMembers());
    }
}
