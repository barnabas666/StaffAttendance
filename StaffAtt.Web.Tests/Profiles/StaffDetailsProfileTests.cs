using AutoMapper;
using StaffAtt.Web.Models;
using StaffAttLibrary.Models;
using FluentAssertions;
using StaffAtt.Web.Profiles;

namespace StaffAtt.Web.Tests.Profiles;
public class StaffDetailsProfileTests
{
    [Fact]
    public void TestStaffDetailsMapping()
    {
        // Arrange
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<StaffDetailsProfile>();
            cfg.AddProfile<AddressProfile>();
        });
        var mapper = config.CreateMapper();
        var staffFullModel = new StaffFullModel
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john.doe@johndoe.com",
            Alias = "JD",
            IsApproved = true,
            DepartmentId = 2,
            Title = "Mr.",
            Address = new AddressModel
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "CA",
                Zip = "12345"
            },
            PhoneNumbers = new List<PhoneNumberModel>
            {
                new PhoneNumberModel { Id = 1, PhoneNumber = "123456789" },
                new PhoneNumberModel { Id = 2, PhoneNumber = "987654321" }
            }
        };
        // Act
        var staffDetailsViewModel = mapper.Map<StaffDetailsViewModel>(staffFullModel);
        // Assert
        staffDetailsViewModel.Should().BeEquivalentTo(staffFullModel,
                                                      options => options.ExcludingMissingMembers());
    }
}
