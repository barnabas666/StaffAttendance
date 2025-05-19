using AutoMapper;
using StaffAtt.Web.Models;
using StaffAttLibrary.Models;
using FluentAssertions;
using StaffAtt.Web.Profiles;

namespace StaffAtt.Web.Tests.Profiles;
public class StaffBasicProfileTests
{
    [Fact]
    public void TestStaffBasicMapping()
    {
        // Arrange
        var config = new MapperConfiguration(cfg => cfg.AddProfile<StaffBasicProfile>());
        var mapper = config.CreateMapper();
        var staffBasicModel = new StaffBasicModel
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
