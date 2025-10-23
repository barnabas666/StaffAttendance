using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using StaffAttApi.Controllers;
using StaffAttLibrary.Data;
using StaffAttLibrary.Models;
using StaffAttShared.DTOs;

namespace StaffAttApi.Tests.Controllers;
public class CheckInControllerTests
{
    private readonly CheckInController _sut;
    private readonly Mock<ICheckInService> _checkInServiceMock = new();
    private readonly Mock<ILogger<CheckInController>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    public CheckInControllerTests()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"Caching:ShortDuration", "10"},
            {"Caching:MediumDuration", "30"},
            {"Caching:LongDuration", "120"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _sut = new CheckInController(
            _checkInServiceMock.Object,
            _loggerMock.Object,
            _mapperMock.Object,
            configuration);
    }

    [Fact]
    public async Task GetLastCheckIn_ReturnsOk_WithCheckInDto_WhenStaffExists()
    {
        // Arrange
        int staffId = 5;
        var checkInModel = new CheckInModel { Id = 1, StaffId = staffId };
        var checkInDto = new CheckInDto { Id = 1, StaffId = staffId };

        _checkInServiceMock.Setup(s => s.GetLastCheckInAsync(staffId))
                           .ReturnsAsync(checkInModel);

        _mapperMock.Setup(m => m.Map<CheckInDto>(checkInModel))
                   .Returns(checkInDto);

        // Act
        var result = await _sut.GetLastCheckIn(staffId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(checkInDto);
    }

    [Fact]
    public async Task GetLastCheckIn_ReturnsOk_WithNull_WhenNoCheckInFound()
    {
        // Arrange
        int staffId = 10;

        _checkInServiceMock.Setup(s => s.GetLastCheckInAsync(staffId))
                           .ReturnsAsync((CheckInModel?)null);

        // Act
        var result = await _sut.GetLastCheckIn(staffId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeNull(); // explicitly null OK body
    }

    [Fact]
    public async Task GetLastCheckIn_ReturnsBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        int staffId = 15;

        _checkInServiceMock.Setup(s => s.GetLastCheckInAsync(staffId))
                           .ThrowsAsync(new Exception("Database down"));

        // Act
        var result = await _sut.GetLastCheckIn(staffId);

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task DoCheckInOrCheckOut_ReturnsOk_WhenOperationSucceeds()
    {
        // Arrange
        int staffId = 5;
        _checkInServiceMock
            .Setup(s => s.DoCheckInOrCheckOutAsync(staffId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DoCheckInOrCheckOut(staffId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { StaffId = staffId });
        _checkInServiceMock.Verify(s => s.DoCheckInOrCheckOutAsync(staffId), Times.Once);
    }

    [Fact]
    public async Task DoCheckInOrCheckOut_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        int staffId = 5;
        _checkInServiceMock
            .Setup(s => s.DoCheckInOrCheckOutAsync(staffId))
            .ThrowsAsync(new Exception("Test failure"));

        // Act
        var result = await _sut.DoCheckInOrCheckOut(staffId);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
        _checkInServiceMock.Verify(s => s.DoCheckInOrCheckOutAsync(staffId), Times.Once);
    }

    [Fact]
    public async Task GetAllCheckInsByDate_ReturnsOk_WithMappedDtos_WhenDataExists()
    {
        // Arrange
        DateTime startDate = new(2025, 9, 1);
        DateTime endDate = new(2025, 9, 22);

        var checkIns = new List<CheckInFullModel>
        {
        new CheckInFullModel { Id = 1, StaffId = 10, FirstName = "John", LastName = "Doe" }
        };
        var checkInDtos = new List<CheckInFullDto>
        {
        new CheckInFullDto { Id = 1, StaffId = 10, FirstName = "John", LastName = "Doe" }
        };

        _checkInServiceMock
            .Setup(s => s.GetAllCheckInsByDateAsync(startDate, endDate))
            .ReturnsAsync(checkIns);

        _mapperMock
            .Setup(m => m.Map<List<CheckInFullDto>>(checkIns))
            .Returns(checkInDtos);

        // Act
        var result = await _sut.GetAllCheckInsByDate(startDate, endDate);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(checkInDtos);
        _checkInServiceMock.Verify(s => s.GetAllCheckInsByDateAsync(startDate, endDate), Times.Once);
        _mapperMock.Verify(m => m.Map<List<CheckInFullDto>>(checkIns), Times.Once);
    }

    [Fact]
    public async Task GetAllCheckInsByDate_ReturnsNotFound_WhenNoDataFound()
    {
        // Arrange
        DateTime startDate = new(2025, 9, 1);
        DateTime endDate = new(2025, 9, 22);

        _checkInServiceMock
            .Setup(s => s.GetAllCheckInsByDateAsync(startDate, endDate))
            .ReturnsAsync((List<CheckInFullModel>?)null);

        // Act
        var result = await _sut.GetAllCheckInsByDate(startDate, endDate);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
        _checkInServiceMock.Verify(s => s.GetAllCheckInsByDateAsync(startDate, endDate), Times.Once);
        _mapperMock.Verify(m => m.Map<List<CheckInFullDto>>(It.IsAny<List<CheckInFullModel>>()), Times.Never);
    }

    [Fact]
    public async Task GetAllCheckInsByDate_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        DateTime startDate = new(2025, 9, 1);
        DateTime endDate = new(2025, 9, 22);

        _checkInServiceMock
            .Setup(s => s.GetAllCheckInsByDateAsync(startDate, endDate))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.GetAllCheckInsByDate(startDate, endDate);

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
        _checkInServiceMock.Verify(s => s.GetAllCheckInsByDateAsync(startDate, endDate), Times.Once);
    }

    [Fact]
    public async Task GetCheckInsByDateAndEmail_ReturnsOk_WithMappedDtos_WhenDataExists()
    {
        // Arrange
        string email = "john.doe@example.com";
        DateTime startDate = new(2025, 9, 1);
        DateTime endDate = new(2025, 9, 22);

        var checkIns = new List<CheckInFullModel>
        {
        new CheckInFullModel { Id = 1, StaffId = 10, FirstName = "John", LastName = "Doe", EmailAddress = email }
        };
        var checkInDtos = new List<CheckInFullDto>
        {
        new CheckInFullDto { Id = 1, StaffId = 10, FirstName = "John", LastName = "Doe", EmailAddress = email }
        };

        _checkInServiceMock
            .Setup(s => s.GetCheckInsByDateAndEmailAsync(email, startDate, endDate))
            .ReturnsAsync(checkIns);

        _mapperMock
            .Setup(m => m.Map<List<CheckInFullDto>>(checkIns))
            .Returns(checkInDtos);

        // Act
        var result = await _sut.GetCheckInsByDateAndEmail(email, startDate, endDate);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(checkInDtos);
        _checkInServiceMock.Verify(s => s.GetCheckInsByDateAndEmailAsync(email, startDate, endDate), Times.Once);
        _mapperMock.Verify(m => m.Map<List<CheckInFullDto>>(checkIns), Times.Once);
    }

    [Fact]
    public async Task GetCheckInsByDateAndEmail_ReturnsNotFound_WhenNoDataFound()
    {
        // Arrange
        string email = "nonexistent@example.com";
        DateTime startDate = new(2025, 9, 1);
        DateTime endDate = new(2025, 9, 22);

        _checkInServiceMock
            .Setup(s => s.GetCheckInsByDateAndEmailAsync(email, startDate, endDate))
            .ReturnsAsync((List<CheckInFullModel>?)null);

        // Act
        var result = await _sut.GetCheckInsByDateAndEmail(email, startDate, endDate);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
        _checkInServiceMock.Verify(s => s.GetCheckInsByDateAndEmailAsync(email, startDate, endDate), Times.Once);
        _mapperMock.Verify(m => m.Map<List<CheckInFullDto>>(It.IsAny<List<CheckInFullModel>>()), Times.Never);
    }

    [Fact]
    public async Task GetCheckInsByDateAndEmail_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        string email = "error@example.com";
        DateTime startDate = new(2025, 9, 1);
        DateTime endDate = new(2025, 9, 22);

        _checkInServiceMock
            .Setup(s => s.GetCheckInsByDateAndEmailAsync(email, startDate, endDate))
            .ThrowsAsync(new Exception("Database failure"));

        // Act
        var result = await _sut.GetCheckInsByDateAndEmail(email, startDate, endDate);

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
        _checkInServiceMock.Verify(s => s.GetCheckInsByDateAndEmailAsync(email, startDate, endDate), Times.Once);
    }

    [Fact]
    public async Task GetCheckInsByDateAndId_ReturnsOk_WithMappedDtos_WhenDataExists()
    {
        // Arrange
        int id = 5;
        DateTime startDate = new(2025, 9, 1);
        DateTime endDate = new(2025, 9, 22);

        var checkIns = new List<CheckInFullModel>
        {
        new CheckInFullModel { Id = 1, StaffId = id, FirstName = "John", LastName = "Doe", EmailAddress = "john@example.com" }
        };
        var checkInDtos = new List<CheckInFullDto>
        {
        new CheckInFullDto { Id = 1, StaffId = id, FirstName = "John", LastName = "Doe", EmailAddress = "john@example.com" }
        };

        _checkInServiceMock
            .Setup(s => s.GetCheckInsByDateAndIdAsync(id, startDate, endDate))
            .ReturnsAsync(checkIns);

        _mapperMock
            .Setup(m => m.Map<List<CheckInFullDto>>(checkIns))
            .Returns(checkInDtos);

        // Act
        var result = await _sut.GetCheckInsByDateAndId(id, startDate, endDate);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(checkInDtos);

        _checkInServiceMock.Verify(s => s.GetCheckInsByDateAndIdAsync(id, startDate, endDate), Times.Once);
        _mapperMock.Verify(m => m.Map<List<CheckInFullDto>>(checkIns), Times.Once);
    }

    [Fact]
    public async Task GetCheckInsByDateAndId_ReturnsNotFound_WhenNoDataFound()
    {
        // Arrange
        int id = 10;
        DateTime startDate = new(2025, 9, 1);
        DateTime endDate = new(2025, 9, 22);

        _checkInServiceMock
            .Setup(s => s.GetCheckInsByDateAndIdAsync(id, startDate, endDate))
            .ReturnsAsync((List<CheckInFullModel>?)null);

        // Act
        var result = await _sut.GetCheckInsByDateAndId(id, startDate, endDate);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();

        _checkInServiceMock.Verify(s => s.GetCheckInsByDateAndIdAsync(id, startDate, endDate), Times.Once);
        _mapperMock.Verify(m => m.Map<List<CheckInFullDto>>(It.IsAny<List<CheckInFullModel>>()), Times.Never);
    }

    [Fact]
    public async Task GetCheckInsByDateAndId_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        int id = 42;
        DateTime startDate = new(2025, 9, 1);
        DateTime endDate = new(2025, 9, 22);

        _checkInServiceMock
            .Setup(s => s.GetCheckInsByDateAndIdAsync(id, startDate, endDate))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.GetCheckInsByDateAndId(id, startDate, endDate);

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
        _checkInServiceMock.Verify(s => s.GetCheckInsByDateAndIdAsync(id, startDate, endDate), Times.Once);
    }
}
