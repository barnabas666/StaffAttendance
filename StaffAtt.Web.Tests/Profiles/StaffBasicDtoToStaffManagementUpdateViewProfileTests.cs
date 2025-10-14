using AutoMapper;
using FluentAssertions;
using StaffAtt.Web.Models;
using StaffAtt.Web.Profiles;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Tests.Profiles;
public class StaffBasicDtoToStaffManagementUpdateViewProfileTests
{
    [Fact]
    public void TestStaffManagementUpdateMapping()
    {
        // Arrange
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<StaffBasicDtoToStaffManagementUpdateViewProfile>();
        });
        var mapper = config.CreateMapper();
        var staffBasicModel = new StaffBasicDto
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


