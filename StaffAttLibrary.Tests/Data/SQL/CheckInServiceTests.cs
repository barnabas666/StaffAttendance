using FluentAssertions;
using Moq;
using StaffAttLibrary.Data;
using StaffAttLibrary.Data.SQL;
using StaffAttLibrary.Db.SQL;
using StaffAttLibrary.Models;

namespace StaffAttLibrary.Tests.Data.SQL;
public class CheckInServiceTests
{
    private readonly CheckInService _sut;
    private readonly Mock<ISqlDataAccess> _dbMock = new();
    private readonly Mock<ICheckInData> _checkInDataMock = new();
    private readonly Mock<IConnectionStringData> _connectionStringMock = new();

    public CheckInServiceTests()
    {
        _sut = new CheckInService(_dbMock.Object, _checkInDataMock.Object, _connectionStringMock.Object);
    }

    [Fact]
    public async Task GetLastCheckInAsync_ShouldReturnCheckInModel_WhenStaffIdIsValid()
    {
        // Arrange
        int staffId = 1;
        var expectedCheckIn = new CheckInModel
        {
            Id = 1,
            StaffId = staffId,
            CheckInDate = DateTime.Now,
            CheckOutDate = null
        };
        _dbMock.Setup(db => db.LoadDataAsync<CheckInModel, dynamic>(
                "spCheckIns_GetLastRecord", It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(new List<CheckInModel> { expectedCheckIn });
        // Act
        var result = await _sut.GetLastCheckInAsync(staffId);
        // Assert
        result.Should().BeEquivalentTo(expectedCheckIn);
    }

    [Fact]
    public async Task GetAllCheckInsByDateAsync_ShouldReturnListOfCheckInFullModel()
    {
        // Arrange
        DateTime startDate = DateTime.Now.AddDays(-1);
        DateTime endDate = DateTime.Now;
        var expectedCheckIns = new List<CheckInFullModel>
        {
            new CheckInFullModel
            {
                Id = 1,
                StaffId = 1,
                CheckInDate = DateTime.Now,
                CheckOutDate = null
            }
        };
        _dbMock.Setup(db => db.LoadDataAsync<CheckInFullModel, dynamic>(
                "spCheckIns_GetAllByDate", It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(expectedCheckIns);
        // Act
        var result = await _sut.GetAllCheckInsByDateAsync(startDate, endDate);
        // Assert
        result.Should().BeEquivalentTo(expectedCheckIns);
    }

    [Fact]
    public async Task GetCheckInsByDateAndEmailAsync_ShouldReturnListOfCheckInFullModel()
    {
        // Arrange
        string emailAddrress = "john.doe@johndoe.com";
        DateTime startDate = DateTime.Now.AddDays(-1);
        DateTime endDate = DateTime.Now;
        var expectedCheckIns = new List<CheckInFullModel>
        {
            new CheckInFullModel
            {
                Id = 1,
                StaffId = 1,
                CheckInDate = DateTime.Now,
                CheckOutDate = null
            }
        };
        _dbMock.Setup(db => db.LoadDataAsync<CheckInFullModel, dynamic>(
                "spCheckIns_GetByDateAndEmail", It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(expectedCheckIns);
        // Act
        var result = await _sut.GetCheckInsByDateAndEmailAsync(emailAddrress, startDate, endDate);
        // Assert
        result.Should().BeEquivalentTo(expectedCheckIns);
    }

    [Fact]
    public async Task GetCheckInsByDateAndIdAsync_ShouldReturnListOfCheckInFullModel()
    {
        // Arrange
        int staffId = 1;
        DateTime startDate = DateTime.Now.AddDays(-1);
        DateTime endDate = DateTime.Now;
        var expectedCheckIns = new List<CheckInFullModel>
        {
            new CheckInFullModel
            {
                Id = 1,
                StaffId = staffId,
                CheckInDate = DateTime.Now,
                CheckOutDate = null
            }
        };
        _dbMock.Setup(db => db.LoadDataAsync<CheckInFullModel, dynamic>(
                "spCheckIns_GetByDateAndId", It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(expectedCheckIns);
        // Act
        var result = await _sut.GetCheckInsByDateAndIdAsync(staffId, startDate, endDate);
        // Assert
        result.Should().BeEquivalentTo(expectedCheckIns);
    }
}
