using FluentAssertions;
using Moq;
using StaffAttLibrary.Data;
using StaffAttLibrary.Db;
using StaffAttLibrary.Models;
using StaffAttLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Tests.Data;
public class CheckInSqliteDataTests
{
    private readonly CheckInSqliteData _sut;
    private readonly Mock<ISqliteDataAccess> _dbMock = new();
    private readonly Mock<IConnectionStringData> _connectionStringMock = new();
    public CheckInSqliteDataTests()
    {
        _sut = new CheckInSqliteData(_dbMock.Object, _connectionStringMock.Object);
    }

    [Fact]
    public async Task CheckInPerformAsync_ShouldReturnCheckInId()
    {
        // Arrange
        int staffId = 1;
        int expectedId = 1;
        string sql = await SqliteQueryHelper.LoadQueryAsync("CheckIns_InsertCheckIn.sql");
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
        string sql = await SqliteQueryHelper.LoadQueryAsync("CheckIns_InsertCheckOut.sql");
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
        string sql = await SqliteQueryHelper.LoadQueryAsync("CheckIns_Delete.sql");
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
