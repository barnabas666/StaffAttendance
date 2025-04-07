using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StaffAttLibrary.Data;
using Moq;
using FluentAssertions;
using StaffAttLibrary.Db;
using StaffAttLibrary.Models;

namespace StaffAttLibrary.Tests.Data;
public class StaffDataProcessorTests
{
    private readonly StaffDataProcessor _sut;
    private readonly Mock<ISqlDataAccess> _dbMock = new();
    private readonly Mock<IConnectionStringData> _connectionStringMock = new();

    public StaffDataProcessorTests()
    {
        _sut = new StaffDataProcessor(_dbMock.Object, _connectionStringMock.Object);
    }

    [Fact]
    public async Task CreatePhoneNumberAsync_ShouldReturnPhoneNumberId()
    {
        // Arrange
        var phoneNumber = new PhoneNumberModel { PhoneNumber = "1234567890" };
        int expectedId = 1;
        _dbMock.Setup(db => db.SaveDataGetIdAsync("spPhoneNumbers_Insert",
                                                  It.IsAny<object>(),
                                                  It.IsAny<string>()))
            .ReturnsAsync(expectedId);
        // Act
        var result = await _sut.CreatePhoneNumberAsync(0, phoneNumber);
        // Assert
        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task CreatePhoneNumberLinkAsync_ShouldCallSaveDataAsync()
    {
        // Arrange
        int staffId = 1;
        int phoneNumberId = 2;
        _dbMock.Setup(db => db.SaveDataAsync("spStaffPhoneNumbers_Insert",
                                              It.IsAny<object>(),
                                              It.IsAny<string>()));
        // Act
        await _sut.CreatePhoneNumberLinkAsync(staffId, phoneNumberId);
        // Assert
        _dbMock.Verify(db => db.SaveDataAsync("spStaffPhoneNumbers_Insert",
                                               It.IsAny<object>(),
                                               It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetByPhoneNumberAsync_ShouldReturnPhoneNumbers()
    {
        // Arrange
        var phoneNumber = new PhoneNumberModel { PhoneNumber = "1234567890" };
        var expectedPhoneNumbers = new List<PhoneNumberModel> { phoneNumber };
        _dbMock.Setup(db => db.LoadDataAsync<PhoneNumberModel, dynamic>(
                "spPhoneNumbers_GetByPhoneNumber",
                It.IsAny<object>(),
                It.IsAny<string>()))
            .ReturnsAsync(expectedPhoneNumbers);
        // Act
        var result = await _sut.GetByPhoneNumberAsync(phoneNumber);
        // Assert
        result.Should().BeEquivalentTo(expectedPhoneNumbers);
    }

    [Fact]
    public async Task DeletePhoneNumberAsync_ShouldCallSaveDataAsync()
    {
        // Arrange
        int phoneNumberId = 1;
        _dbMock.Setup(db => db.SaveDataAsync("spPhoneNumbers_Delete",
                                              It.IsAny<object>(),
                                              It.IsAny<string>()));
        // Act
        await _sut.DeletePhoneNumberAsync(phoneNumberId);
        // Assert
        _dbMock.Verify(db => db.SaveDataAsync("spPhoneNumbers_Delete",
                                               It.IsAny<object>(),
                                               It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task DeletePhoneNumberLinkAsync_ShouldCallSaveDataAsync()
    {
        // Arrange
        int staffId = 1;
        int phoneNumberId = 2;
        _dbMock.Setup(db => db.SaveDataAsync("spStaffPhoneNumbers_Delete",
                                              It.IsAny<object>(),
                                              It.IsAny<string>()));
        // Act
        await _sut.DeletePhoneNumberLinkAsync(staffId, phoneNumberId);
        // Assert
        _dbMock.Verify(db => db.SaveDataAsync("spStaffPhoneNumbers_Delete",
                                               It.IsAny<object>(),
                                               It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetPhoneNumberLinksAsync_ShouldReturnPhoneNumberLinks()
    {
        // Arrange
        int phoneNumberId = 1;
        var expectedLinks = new List<StaffPhoneNumberModel>
        {
            new StaffPhoneNumberModel { PhoneNumberId = phoneNumberId, StaffId = 1 }
        };
        _dbMock.Setup(db => db.LoadDataAsync<StaffPhoneNumberModel, dynamic>(
                "spStaffPhoneNumbers_GetByPhoneNumber",
                It.IsAny<object>(),
                It.IsAny<string>()))
            .ReturnsAsync(expectedLinks);
        // Act
        var result = await _sut.GetPhoneNumberLinksAsync(phoneNumberId);
        // Assert
        result.Should().BeEquivalentTo(expectedLinks);
    }
}
