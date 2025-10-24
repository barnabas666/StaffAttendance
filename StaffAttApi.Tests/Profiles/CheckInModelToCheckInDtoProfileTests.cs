using AutoMapper;
using FluentAssertions;
using StaffAttApi.Profiles;
using StaffAttLibrary.Models;
using StaffAttShared.DTOs;

namespace StaffAttApi.Tests.Profiles;
public class CheckInModelToCheckInDtoProfileTests
{
    [Fact]
    public void TestCheckInMapping()
    {
        // Arrange
        var config = new MapperConfiguration(cfg => cfg.AddProfile<CheckInModelToCheckInDtoProfile>());
        var mapper = config.CreateMapper();

        var model = new CheckInModel
        {
            Id = 22,
            StaffId = 7,
            CheckInDate = new DateTime(2025, 10, 2, 8, 30, 0),
            CheckOutDate = new DateTime(2025, 10, 2, 16, 30, 0)
        };

        // Act
        var dto = mapper.Map<CheckInDto>(model);

        // Assert
        dto.Id.Should().Be(model.Id);
        dto.StaffId.Should().Be(model.StaffId);
        dto.CheckInDate.Should().Be(model.CheckInDate);
        dto.CheckOutDate.Should().Be(model.CheckOutDate);
    }
}
