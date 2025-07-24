using FluentAssertions;
using Moq;
using StaffAttLibrary.Data;
using StaffAttLibrary.Db;

namespace StaffAttLibrary.Tests.Data;
public class CheckInDataTests
{
    private readonly CheckInData _sut;
    private readonly Mock<ISqlDataAccess> _dbMock = new();
    private readonly Mock<IConnectionStringData> _connectionStringMock = new();

    public CheckInDataTests()
    {
        _sut = new CheckInData(_dbMock.Object, _connectionStringMock.Object);
    }

    [Fact]
    public async Task CheckInPerformAsync_ShouldReturnCheckInId()
    {
        // Arrange
        int staffId = 1;
        int expectedId = 1;
        _dbMock.Setup(db => db.SaveDataGetIdAsync("spCheckIns_InsertCheckIn",
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
        _dbMock.Setup(db => db.SaveDataGetIdAsync("spCheckIns_InsertCheckOut",
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
        _dbMock.Setup(db => db.SaveDataAsync("spCheckIns_Delete",
                                              It.IsAny<object>(),
                                              It.IsAny<string>()));
        // Act
        await _sut.DeleteCheckInAsync(staffId);
        // Assert
        _dbMock.Verify(db => db.SaveDataAsync("spCheckIns_Delete",
                                               It.IsAny<object>(),
                                               It.IsAny<string>()), Times.Once);
    }
}
