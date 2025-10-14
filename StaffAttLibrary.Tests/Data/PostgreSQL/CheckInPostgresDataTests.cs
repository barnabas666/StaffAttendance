using FluentAssertions;
using Moq;
using StaffAttLibrary.Data;
using StaffAttLibrary.Data.PostgreSQL;
using StaffAttLibrary.Db.PostgreSQL;
using StaffAttLibrary.Helpers;


namespace StaffAttLibrary.Tests.Data.PostgreSQL;
public class CheckInPostgresDataTests
{
    private readonly CheckInPostgresData _sut;
    private readonly Mock<IPostgresDataAccess> _dbMock = new();
    private readonly Mock<IConnectionStringData> _connectionStringMock = new();
    public CheckInPostgresDataTests()
    {
        _sut = new CheckInPostgresData(_dbMock.Object, _connectionStringMock.Object);
    }

    [Fact]
    public async Task CheckInPerformAsync_ShouldReturnCheckInId()
    {
        // Arrange
        int staffId = 1;
        int expectedId = 1;
        string sql = await QueryHelper.LoadPostgresQueryAsync("CheckIns_InsertCheckIn.sql");
        _dbMock.Setup(db => db.SaveDataGetIdAsync(sql,
                                                  It.IsAny<object>(),
                                                  It.IsAny<string>()))
            .ReturnsAsync(expectedId);
        // Act
        var result = await _sut.CheckInPerformAsync(staffId);
        // Assert
        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task CheckOutPerformAsync_ShouldReturnCheckOutId()
    {
        // Arrange
        int checkInId = 1;
        int expectedId = 1;
        string sql = await QueryHelper.LoadPostgresQueryAsync("CheckIns_InsertCheckOut.sql");
        _dbMock.Setup(db => db.SaveDataGetIdAsync(sql,
                                                  It.IsAny<object>(),
                                                  It.IsAny<string>()))
            .ReturnsAsync(expectedId);
        // Act
        var result = await _sut.CheckOutPerformAsync(checkInId);
        // Assert
        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task DeleteCheckInAsync_ShouldCallSaveDataAsync()
    {
        // Arrange
        int staffId = 1;
        string sql = await QueryHelper.LoadPostgresQueryAsync("CheckIns_Delete.sql");
        _dbMock.Setup(db => db.SaveDataAsync(sql,
                                             It.IsAny<object>(),
                                             It.IsAny<string>()));
        // Act
        await _sut.DeleteCheckInAsync(staffId);
        // Assert
        _dbMock.Verify(db => db.SaveDataAsync(sql,
                                              It.IsAny<object>(),
                                              It.IsAny<string>()), Times.Once);
    }
}
