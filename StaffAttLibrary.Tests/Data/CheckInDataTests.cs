using FluentAssertions;
using Moq;
using StaffAttLibrary.Data;
using StaffAttLibrary.Db;
using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public async Task GetBasicStaffByAliasIdAsync_ShouldReturnStaffBasicModel()
    {
        // Arrange
        int aliasId = 1;
        var expectedStaff = new StaffBasicModel
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john.doe@johndoe.com",
            Alias = "JDO1",
            IsApproved = true,
            DepartmentId = 1,
            Title = "IT"
        };
        _dbMock.Setup(db => db.LoadDataAsync<StaffBasicModel, dynamic>(
                "spStaffs_GetBasicByAliasId",
                It.IsAny<object>(),
                It.IsAny<string>()))
            .ReturnsAsync(new List<StaffBasicModel> { expectedStaff });
        // Act
        var result = await _sut.GetBasicStaffByAliasIdAsync(aliasId);
        // Assert
        result.Should().BeEquivalentTo(expectedStaff);
    }

    [Fact]
    public async Task GetBasicStaffByAliasIdAsync_ShouldThrowArgumentException()
    {
        // Arrange
        int aliasId = 1;
        _dbMock.Setup(db => db.LoadDataAsync<StaffBasicModel, dynamic>(
                "spStaffs_GetBasicByAliasId",
                It.IsAny<object>(),
                It.IsAny<string>())).ReturnsAsync(new List<StaffBasicModel>());
        // Act
        Func<Task> act = async () => await _sut.GetBasicStaffByAliasIdAsync(aliasId);
        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("You passed in an invalid parameter (Parameter 'aliasId')");
    }
}
