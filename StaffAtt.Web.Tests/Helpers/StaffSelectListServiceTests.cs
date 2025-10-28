using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using StaffAtt.Web.Helpers;
using StaffAtt.Web.Models;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Tests.Helpers
{
    public class StaffSelectListServiceTests
    {
        private readonly StaffSelectListService _sut;
        private readonly Mock<IApiClient> _apiClientMock = new();
        private readonly Mock<IMapper> _mapperMock = new();

        public StaffSelectListServiceTests()
        {
            _sut = new StaffSelectListService(_apiClientMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetStaffSelectListAsync_ShouldReturnSelectListWithDefaultValue_WhenDefaultValueIsProvided()
        {
            // Arrange
            string defaultValue = "All Staff";
            var checkInDisplayAdminViewModel = new CheckInDisplayAdminViewModel();

            var staffDtos = new List<StaffBasicDto>
            {
                new() { Id = 1, FirstName = "John", LastName = "Doe" },
                new() { Id = 2, FirstName = "Jane", LastName = "Smith" }
            };

            _apiClientMock
                .Setup(x => x.GetAsync<List<StaffBasicDto>>("staff/basic"))
                .ReturnsAsync(Result<List<StaffBasicDto>>.Success(staffDtos));

            var mappedViewModels = new List<StaffBasicViewModel>
            {
                new() { Id = 0, FirstName = defaultValue },
                new() { Id = 1, FirstName = "John", LastName = "Doe" },
                new() { Id = 2, FirstName = "Jane", LastName = "Smith" }
            };

            _mapperMock
                .Setup(x => x.Map<List<StaffBasicViewModel>>(It.IsAny<List<StaffBasicDto>>()))
                .Returns(mappedViewModels);

            // Act
            var result = await _sut.GetStaffSelectListAsync(checkInDisplayAdminViewModel, defaultValue);

            // Assert
            result.Should().NotBeNull();
            var items = result.Items.Cast<StaffBasicViewModel>().ToList();
            items.First().FirstName.Should().Be(defaultValue);
            items.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetStaffSelectListAsync_ShouldReturnSelectListWithoutDefaultValue_WhenDefaultValueIsNotProvided()
        {
            // Arrange
            var checkInDisplayAdminViewModel = new CheckInDisplayAdminViewModel();

            var staffDtos = new List<StaffBasicDto>
            {
                new() { Id = 1, FirstName = "John", LastName = "Doe" },
                new() { Id = 2, FirstName = "Jane", LastName = "Smith" }
            };

            _apiClientMock
                .Setup(x => x.GetAsync<List<StaffBasicDto>>("staff/basic"))
                .ReturnsAsync(Result<List<StaffBasicDto>>.Success(staffDtos));

            var mappedViewModels = new List<StaffBasicViewModel>
            {
                new() { Id = 1, FirstName = "John", LastName = "Doe" },
                new() { Id = 2, FirstName = "Jane", LastName = "Smith" }
            };

            _mapperMock
                .Setup(x => x.Map<List<StaffBasicViewModel>>(staffDtos))
                .Returns(mappedViewModels);

            // Act
            var result = await _sut.GetStaffSelectListAsync(checkInDisplayAdminViewModel);

            // Assert
            result.Should().NotBeNull();
            var items = result.Items.Cast<StaffBasicViewModel>().ToList();
            items.Should().HaveCount(2);
            items.First().FirstName.Should().Be("John");
            items.Last().FirstName.Should().Be("Jane");
        }
    }
}
