using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using StaffAtt.Web.Helpers;
using StaffAttLibrary.Data;
using StaffAttLibrary.Models;

namespace StaffAtt.Web.Tests.Helpers;
public class DepartmentSelectListServiceTests
{
    private readonly DepartmentSelectListService _sut;
    private readonly Mock<IStaffService> _staffServiceMock = new();

    public DepartmentSelectListServiceTests()
    {
        _sut = new DepartmentSelectListService(_staffServiceMock.Object);
    }

    [Fact]
    public async Task GetDepartmentSelectListAsync_ShouldReturnSelectListWithDefaultValue_WhenDefaultValueIsProvided()
    {
        // Arrange
        string defaultValue = "All";
        var departments = new List<DepartmentModel>
        {
            new DepartmentModel { Id = 1, Title = "HR" },
            new DepartmentModel { Id = 2, Title = "IT" }
        };
        _staffServiceMock.Setup(x => x.GetAllDepartmentsAsync()).ReturnsAsync(departments);
        var defaultDepartment = new DepartmentModel { Id = 0, Title = defaultValue };
        departments.Insert(0, defaultDepartment);
        SelectList expectedSelectList = new SelectList(departments, nameof(DepartmentModel.Id), nameof(DepartmentModel.Title));
        // Act
        var result = await _sut.GetDepartmentSelectListAsync(defaultValue);
        // Assert
        result.Should().BeEquivalentTo(expectedSelectList, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetDepartmentSelectListAsync_ShouldReturnSelectListWithoutDefaultValue_WhenDefaultValueIsNotProvided()
    {
        // Arrange
        var departments = new List<DepartmentModel>
        {
            new DepartmentModel { Id = 1, Title = "HR" },
            new DepartmentModel { Id = 2, Title = "IT" }
        };
        _staffServiceMock.Setup(x => x.GetAllDepartmentsAsync()).ReturnsAsync(departments);
        SelectList expectedSelectList = new SelectList(departments, nameof(DepartmentModel.Id), nameof(DepartmentModel.Title));
        // Act
        var result = await _sut.GetDepartmentSelectListAsync();
        // Assert
        result.Should().BeEquivalentTo(expectedSelectList, options => options.WithStrictOrdering());
    }
}
