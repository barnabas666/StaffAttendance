using FluentAssertions;
using Moq;
using StaffAttLibrary.Data;
using StaffAttLibrary.Data.PostgreSQL;
using StaffAttLibrary.Db.PostgreSQL;
using StaffAttLibrary.Helpers;
using StaffAttLibrary.Models;

namespace StaffAttLibrary.Tests.Data.PostgreSQL;
public class CheckInPostgresServiceTests
{
    private readonly CheckInPostgresService _sut;
    private readonly Mock<IPostgresDataAccess> _dbMock = new();
    private readonly Mock<ICheckInData> _checkInDataMock = new();
    private readonly Mock<IConnectionStringData> _connectionStringMock = new();

    public CheckInPostgresServiceTests()
    {
        _sut = new CheckInPostgresService(_dbMock.Object, _checkInDataMock.Object, _connectionStringMock.Object);
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
        string sql = await QueryHelper.LoadPostgresQueryAsync("CheckIns_GetLastRecord.sql");
        _dbMock.Setup(db => db.LoadDataAsync<CheckInModel, dynamic>(
                sql, It.IsAny<object>(), It.IsAny<string>()))
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
        string sql = await QueryHelper.LoadPostgresQueryAsync("CheckIns_GetAllByDate.sql");
        _dbMock.Setup(db => db.LoadDataAsync<CheckInFullModel, dynamic>(
                sql, It.IsAny<object>(), It.IsAny<string>()))
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
        string sql = await QueryHelper.LoadPostgresQueryAsync("CheckIns_GetByDateAndEmail.sql");
        _dbMock.Setup(db => db.LoadDataAsync<CheckInFullModel, dynamic>(
                sql, It.IsAny<object>(), It.IsAny<string>()))
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
        string sql = await QueryHelper.LoadPostgresQueryAsync("CheckIns_GetByDateAndId.sql");
        _dbMock.Setup(db => db.LoadDataAsync<CheckInFullModel, dynamic>(
                sql, It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(expectedCheckIns);
        // Act
        var result = await _sut.GetCheckInsByDateAndIdAsync(staffId, startDate, endDate);
        // Assert
        result.Should().BeEquivalentTo(expectedCheckIns);
    }
}
