using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using StaffAtt.Web.Models;
using StaffAtt.Web.Services;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Tests.Helpers
{
    public class DepartmentSelectListServiceTests
    {
        private readonly DepartmentSelectListService _sut;
        private readonly Mock<IApiClient> _apiClientMock = new();
        private readonly Mock<IMemoryCache> _memoryCacheMock = new();

        public DepartmentSelectListServiceTests()
        {
            _sut = new DepartmentSelectListService(_apiClientMock.Object, _memoryCacheMock.Object);
        }

        [Fact]
        public async Task GetDepartmentSelectListAsync_ShouldReturnSelectListWithDefaultValue_WhenDefaultValueIsProvided()
        {
            // Arrange
            string defaultValue = "All";
            var departments = new List<DepartmentDto>
            {
                new DepartmentDto { Id = 1, Title = "HR" },
                new DepartmentDto { Id = 2, Title = "IT" }
            };

            _apiClientMock
                .Setup(x => x.GetAsync<List<DepartmentDto>>("staff/departments"))
                .ReturnsAsync(Result<List<DepartmentDto>>.Success(departments));

            // Act
            var result = await _sut.GetDepartmentSelectListAsync(defaultValue);

            // Assert
            var selectListItems = result.Cast<SelectListItem>().ToList();

            selectListItems.Should().Contain(x => x.Text == defaultValue && x.Value == "0");
            selectListItems.Should().Contain(x => x.Text == "HR" && x.Value == "1");
            selectListItems.Should().Contain(x => x.Text == "IT" && x.Value == "2");
        }

        [Fact]
        public async Task GetDepartmentSelectListAsync_ShouldReturnSelectListWithoutDefaultValue_WhenDefaultValueIsNotProvided()
        {
            // Arrange
            var departments = new List<DepartmentDto>
            {
                new DepartmentDto { Id = 1, Title = "HR" },
                new DepartmentDto { Id = 2, Title = "IT" }
            };

            _apiClientMock
                .Setup(x => x.GetAsync<List<DepartmentDto>>("staff/departments"))
                .ReturnsAsync(Result<List<DepartmentDto>>.Success(departments));

            // Act
            var result = await _sut.GetDepartmentSelectListAsync();

            // Assert
            var selectListItems = result.Cast<SelectListItem>().ToList();

            selectListItems.Should().HaveCount(2);
            selectListItems.Should().Contain(x => x.Text == "HR" && x.Value == "1");
            selectListItems.Should().Contain(x => x.Text == "IT" && x.Value == "2");
        }

        [Fact]
        public async Task GetDepartmentSelectListAsync_ShouldReturnEmptySelectList_WhenApiCallFails()
        {
            // Arrange
            _apiClientMock
                .Setup(x => x.GetAsync<List<DepartmentDto>>("staff/departments"))
                .ReturnsAsync(Result<List<DepartmentDto>>.Failure("API error"));

            // Act
            var result = await _sut.GetDepartmentSelectListAsync();

            // Assert
            var selectListItems = result.Cast<SelectListItem>().ToList();
            selectListItems.Should().BeEmpty();
        }
    }
}
