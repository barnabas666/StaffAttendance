using StaffAttLibrary.Models;
using Moq;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StaffAttLibrary.Data;
using StaffAtt.Web.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using StaffAtt.Web.Models;

namespace StaffAtt.Web.Tests.Helpers;
public class StaffSelectListServiceTests
{
    private readonly StaffSelectListService _sut;
    private readonly Mock<IStaffService> _staffServiceMock = new();

    public StaffSelectListServiceTests()
    {
        _sut = new StaffSelectListService(_staffServiceMock.Object);
    }

    [Fact]
    public async Task GetStaffSelectListAsync_ShouldReturnSelectListWithDefaultValue_WhenDefaultValueIsProvided()
    {
        // Arrange
        string defaultValue = "All Staff";
        CheckInDisplayAdminViewModel checkInDisplayAdminViewModel = new CheckInDisplayAdminViewModel();
        checkInDisplayAdminViewModel.StaffList = new List<StaffBasicViewModel>
        {
            new StaffBasicViewModel { Id = 1, FirstName = "John", LastName = "Doe" },
            new StaffBasicViewModel { Id = 2, FirstName = "Jane", LastName = "Smith" }
        };
        var defaultStaff = new StaffBasicViewModel { Id = 0, FirstName = defaultValue };
        checkInDisplayAdminViewModel.StaffList.Insert(0, defaultStaff);
        SelectList expectedSelectList = new SelectList(checkInDisplayAdminViewModel.StaffList, nameof(StaffBasicModel.Id), nameof(StaffBasicModel.FullName));

        // Act
        var result = await _sut.GetStaffSelectListAsync(checkInDisplayAdminViewModel, defaultValue);

        // Assert
        result.Should().BeEquivalentTo(expectedSelectList, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetStaffSelectListAsync_ShouldReturnSelectListWithoutDefaultValue_WhenDefaultValueIsNotProvided()
    {
        // Arrange
        CheckInDisplayAdminViewModel checkInDisplayAdminViewModel = new CheckInDisplayAdminViewModel();
        checkInDisplayAdminViewModel.StaffList = new List<StaffBasicViewModel>
        {
            new StaffBasicViewModel { Id = 1, FirstName = "John", LastName = "Doe" },
            new StaffBasicViewModel { Id = 2, FirstName = "Jane", LastName = "Smith" }
        };
        SelectList expectedSelectList = new SelectList(checkInDisplayAdminViewModel.StaffList, nameof(StaffBasicModel.Id), nameof(StaffBasicModel.FullName));

        // Act
        var result = await _sut.GetStaffSelectListAsync(checkInDisplayAdminViewModel);

        // Assert
        result.Should().BeEquivalentTo(expectedSelectList, options => options.WithStrictOrdering());
    }
}
