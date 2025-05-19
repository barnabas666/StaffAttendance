using AutoMapper;
using StaffAtt.Web.Models;
using StaffAttLibrary.Models;
using FluentAssertions;
using StaffAtt.Web.Profiles;

namespace StaffAtt.Web.Tests.Profiles;
public class CheckInFullProfileTests
{
    [Fact]
    public void TestCheckInFullMapping()
    {
        // Arrange
        var config = new MapperConfiguration(cfg => cfg.AddProfile<CheckInFullProfile>());
        var mapper = config.CreateMapper();
        var checkInFullModel = new CheckInFullModel
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john.doe@johndoe.com",
            Title = "Mr.",
            CheckInDate = DateTime.Now,
            CheckOutDate = DateTime.Now.AddHours(8)
        };
        // Act
        var checkInFullViewModel = mapper.Map<CheckInFullViewModel>(checkInFullModel);
        // Assert
        checkInFullViewModel.Should().BeEquivalentTo(checkInFullModel,
                                                     options => options.ExcludingMissingMembers());
    }
}
