using AutoMapper;
using FluentAssertions;
using StaffAttApi.Profiles;
using StaffAttLibrary.Models;
using StaffAttShared.DTOs;

namespace StaffAttApi.Tests.Profiles;
public class CheckInFullModelToCheckInFullDtoProfileTests
{
    [Fact]
    public void TestCheckInFullMapping()
    {
        // Arrange
        var config = new MapperConfiguration(cfg => cfg.AddProfile<CheckInFullModelToCheckInFullDtoProfile>());
        var mapper = config.CreateMapper();

        var model = new CheckInFullModel
        {
            Id = 11,
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john.doe@example.com",
            Title = "Developer",
            StaffId = 5,
            CheckInDate = new DateTime(2025, 10, 1, 8, 0, 0),
            CheckOutDate = new DateTime(2025, 10, 1, 16, 0, 0)
        };

        // Act
        var dto = mapper.Map<CheckInFullDto>(model);

        // Assert
        dto.Id.Should().Be(model.Id);
        dto.FirstName.Should().Be(model.FirstName);
        dto.LastName.Should().Be(model.LastName);
        dto.EmailAddress.Should().Be(model.EmailAddress);
        dto.Title.Should().Be(model.Title);
        dto.StaffId.Should().Be(model.StaffId);
        dto.CheckInDate.Should().Be(model.CheckInDate);
        dto.CheckOutDate.Should().Be(model.CheckOutDate);
    }
}
