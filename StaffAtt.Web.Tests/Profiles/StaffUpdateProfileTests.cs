using AutoMapper;
using FluentAssertions;
using StaffAtt.Web.Models;
using StaffAtt.Web.Profiles;
using StaffAttLibrary.Models;

namespace StaffAtt.Web.Tests.Profiles;
public class StaffUpdateProfileTests
{
    [Fact]
    public void TestStaffUpdateMapping()
    {
        // Arrange
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<StaffUpdateViewProfile>();
            cfg.AddProfile<AddressProfile>();
        });
        var mapper = config.CreateMapper();
        var staffFullModel = new StaffFullModel
        {
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john.doe@johndoe.com",
            Address = new AddressModel
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
