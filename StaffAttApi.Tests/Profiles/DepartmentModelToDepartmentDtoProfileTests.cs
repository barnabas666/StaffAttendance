using AutoMapper;
using FluentAssertions;
using StaffAttApi.Profiles;
using StaffAttLibrary.Models;
using StaffAttShared.DTOs;

namespace StaffAttApi.Tests.Profiles;
public class DepartmentModelToDepartmentDtoProfileTests
{
    [Fact]
    public void TestDepartmentMapping()
    {
        // Arrange
        var config = new MapperConfiguration(cfg => cfg.AddProfile<DepartmentModelToDepartmentDtoProfile>());
        var mapper = config.CreateMapper();

        var departmentModel = new DepartmentModel
        {
            Id = 5,
            Title = "Human Resources",
            Description = "Responsible for employee relations"
        };

        // Act
        var departmentDto = mapper.Map<DepartmentDto>(departmentModel);

        // Assert
        departmentDto.Should().BeEquivalentTo(departmentModel, options => options.ExcludingMissingMembers());
    }
}
