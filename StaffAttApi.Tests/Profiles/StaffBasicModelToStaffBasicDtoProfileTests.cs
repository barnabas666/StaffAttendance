using AutoMapper;
using FluentAssertions;
using StaffAttApi.Profiles;
using StaffAttLibrary.Models;
using StaffAttShared.DTOs;

namespace StaffAttApi.Tests.Profiles;
public class StaffBasicModelToStaffBasicDtoProfileTests
{
    [Fact]
    public void TestStaffBasicMapping()
    {
        // Arrange
        var config = new MapperConfiguration(cfg => cfg.AddProfile<StaffBasicModelToStaffBasicDtoProfile>());
        var mapper = config.CreateMapper();

        var model = new StaffBasicModel
        {
            Id = 3,
            FirstName = "Alice",
            LastName = "Smith",
            EmailAddress = "alice.smith@example.com",
            Alias = "AS",
            IsApproved = true,
            DepartmentId = 2,
            Title = "HR Specialist"
        };

        // Act
        var dto = mapper.Map<StaffBasicDto>(model);

        // Assert
        dto.Id.Should().Be(model.Id);
        dto.FirstName.Should().Be(model.FirstName);
        dto.LastName.Should().Be(model.LastName);
        dto.EmailAddress.Should().Be(model.EmailAddress);
        dto.Alias.Should().Be(model.Alias);
        dto.IsApproved.Should().Be(model.IsApproved);
        dto.DepartmentId.Should().Be(model.DepartmentId);
        dto.Title.Should().Be(model.Title);
    }
}
