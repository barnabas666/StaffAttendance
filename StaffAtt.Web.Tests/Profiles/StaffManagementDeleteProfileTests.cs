using AutoMapper;
using StaffAtt.Web.Models;
using StaffAttLibrary.Models;
using FluentAssertions;
using StaffAtt.Web.Profiles;

namespace StaffAtt.Web.Tests.Profiles;
public class StaffManagementDeleteProfileTests
{
    [Fact]
    public void TestStaffManagementDeleteMapping()
    {
        // Arrange
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<StaffManagementDeleteProfile>();            
        });
        var mapper = config.CreateMapper();
        var staffBasicModel = new StaffBasicModel
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john.doe@johndoe.com",
            Alias = "JD",
            IsApproved = true,
            DepartmentId = 2,
            Title = "Mr."
        };
        // Act
        var staffManagementDeleteViewModel = mapper.Map<StaffManagementDeleteViewModel>(staffBasicModel);
        // Assert
        staffManagementDeleteViewModel.Should().BeEquivalentTo(staffBasicModel,
                                                               options => options.ExcludingMissingMembers());
    }
}
