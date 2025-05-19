using AutoMapper;
using StaffAtt.Web.Models;
using StaffAttLibrary.Models;
using FluentAssertions;
using StaffAtt.Web.Profiles;

namespace StaffAtt.Web.Tests.Profiles;
public class StaffManagementUpdateProfileTests
{
    [Fact]
    public void TestStaffManagementUpdateMapping()
    {
        // Arrange
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<StaffManagementUpdateProfile>();            
        });
        var mapper = config.CreateMapper();
        var staffBasicModel = new StaffBasicModel
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john.doe@johndoe.com",
            Alias = "jdoe",
            IsApproved = true,
            DepartmentId = 2,
            Title = "IT"
        };
        // Act
        var staffManagementUpdateViewModel = mapper.Map<StaffManagementUpdateViewModel>(staffBasicModel);
        // Assert
        staffManagementUpdateViewModel.Should().BeEquivalentTo(staffBasicModel,
                                                               options => options.ExcludingMissingMembers());
    }
}


