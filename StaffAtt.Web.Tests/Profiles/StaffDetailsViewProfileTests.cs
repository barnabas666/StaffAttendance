using AutoMapper;
using FluentAssertions;
using StaffAtt.Web.Models;
using StaffAtt.Web.Profiles;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Tests.Profiles;
public class StaffDetailsViewProfileTests
{
    [Fact]
    public void TestStaffDetailsViewMapping()
    {
        // Arrange
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<StaffDetailsViewProfile>();
            cfg.AddProfile<AddressDtoProfile>();
        });
        var mapper = config.CreateMapper();
        var staffFullModel = new StaffFullDto
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john.doe@johndoe.com",
            Alias = "JD",
            IsApproved = true,
            DepartmentId = 2,
            Title = "Mr.",
            Address = new AddressDto
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "CA",
                Zip = "12345"
            },
            PhoneNumbers = new List<PhoneNumberDto>
            {
                new PhoneNumberDto { Id = 1, PhoneNumber = "123456789" },
                new PhoneNumberDto { Id = 2, PhoneNumber = "987654321" }
            }
        };
        // Act
        var staffDetailsViewModel = mapper.Map<StaffDetailsViewModel>(staffFullModel);
        // Assert
        staffDetailsViewModel.Should().BeEquivalentTo(staffFullModel,
                                                      options => options.ExcludingMissingMembers());
    }
}
