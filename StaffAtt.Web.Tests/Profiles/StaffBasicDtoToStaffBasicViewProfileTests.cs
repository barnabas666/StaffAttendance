using AutoMapper;
using FluentAssertions;
using StaffAtt.Web.Models;
using StaffAtt.Web.Profiles;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Tests.Profiles;
public class StaffBasicDtoToStaffBasicViewProfileTests
{
    [Fact]
    public void TestStaffBasicMapping()
    {
        // Arrange
        var config = new MapperConfiguration(cfg => cfg.AddProfile<StaffBasicDtoToStaffBasicViewProfile>());
        var mapper = config.CreateMapper();
        var staffBasicModel = new StaffBasicDto
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john.doe@johndoe.com",
            Alias = "JD",
            IsApproved = true,
            Title = "Mr."
        };
        // Act
        var staffBasicViewModel = mapper.Map<StaffBasicViewModel>(staffBasicModel);
        // Assert
        staffBasicViewModel.Should().BeEquivalentTo(staffBasicModel,
                                                    options => options.ExcludingMissingMembers());
    }
}
