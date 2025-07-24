using FluentAssertions;
using Moq;
using StaffAttLibrary.Data;
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
    public async Task SavePhoneNumberAsync_ShouldReturnPhoneNumberId()
    {
        // Arrange
        var phoneNumber = new PhoneNumberModel { PhoneNumber = "1234567890" };
        int expectedId = 1;
        _dbMock.Setup(db => db.SaveDataGetIdAsync("spPhoneNumbers_Insert",
                                                  It.IsAny<object>(),
                                                  It.IsAny<string>()))
            .ReturnsAsync(expectedId);
        // Act
        var result = await _sut.SavePhoneNumberAsync(phoneNumber);
        // Assert
        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task SavePhoneNumberLinkAsync_ShouldCallSaveDataAsync()
    {
        // Arrange
        int staffId = 1;
        int phoneNumberId = 2;
        _dbMock.Setup(db => db.SaveDataAsync("spStaffPhoneNumbers_Insert",
                                              It.IsAny<object>(),
                                              It.IsAny<string>()));
        // Act
        await _sut.SavePhoneNumberLinkAsync(staffId, phoneNumberId);
        // Assert
        _dbMock.Verify(db => db.SaveDataAsync("spStaffPhoneNumbers_Insert",
                                               It.IsAny<object>(),
                                               It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task CheckPhoneNumberAsync_ShouldReturnTrue_WhenPhoneNumberExists()
    {
        // Arrange
        var phoneNumber = new PhoneNumberModel { PhoneNumber = "123456789" };
        _dbMock.Setup(db => db.LoadDataAsync<bool, dynamic>(
                "spPhoneNumbers_Check",
                It.IsAny<object>(),
                It.IsAny<string>()))
            .ReturnsAsync(new List<bool> { true });
        // Act
        var result = await _sut.CheckPhoneNumberAsync(phoneNumber);
        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetPhoneNumberIdAsync_ShouldReturnPhoneNumberIds()
    {
        // Arrange
        var phoneNumber = new PhoneNumberModel { PhoneNumber = "123456789" };
        var expectedPhoneNumberId = 1;
        var PhoneNumberIds = new List<int> { expectedPhoneNumberId };
        _dbMock.Setup(db => db.LoadDataAsync<int, dynamic>(
                "spPhoneNumbers_GetIdByPhoneNumber",
                It.IsAny<object>(),
                It.IsAny<string>()))
            .ReturnsAsync(PhoneNumberIds);
        // Act
        var result = await _sut.GetPhoneNumberIdAsync(phoneNumber);
        // Assert
        result.Should().Be(expectedPhoneNumberId);
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

    [Fact]
    public async Task CheckAliasAsync_ShouldReturnTrue_WhenAliasAlreadyExistsInDb()
    {
        // Arrange
        string alias = "AAD1";
        List<bool> expectedResult = new List<bool> { true };
        _dbMock.Setup(db => db.LoadDataAsync<bool, dynamic>(
                "spAliases_Check",
                It.IsAny<object>(),
                It.IsAny<string>()))
            .ReturnsAsync(expectedResult);
        // Act
        var result = await _sut.CheckAliasAsync(alias);
        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CheckAliasAsync_ShouldReturnFalse_WhenAliasDoesNotExistInDb()
    {
        // Arrange
        string alias = "AAD1";
        List<bool> expectedResult = new List<bool> { false };
        _dbMock.Setup(db => db.LoadDataAsync<bool, dynamic>(
                "spAliases_Check",
                It.IsAny<object>(),
                It.IsAny<string>()))
            .ReturnsAsync(expectedResult);
        // Act
        var result = await _sut.CheckAliasAsync(alias);
        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SaveAliasAsync_ShouldReturnCreatedAliasId()
    {
        // Arrange
        string pIN = "1234";
        string alias = "AAD1";
        int expectedId = 1;
        _dbMock.Setup(db => db.SaveDataGetIdAsync("spAliases_Insert",
                                                  It.IsAny<object>(),
                                                  It.IsAny<string>()))
            .ReturnsAsync(expectedId);
        // Act
        var result = await _sut.SaveAliasAsync(pIN, alias);
        // Assert
        result.Should().Be(expectedId);
    }

    [Fact]
    public void CreateAlias_ShouldReturnAlias()
    {
        // Arrange
        string firstName = "John";
        string lastName = "Doe";
        int orderNumber = 1;
        string expectedAlias = "JDO1";
        // Act
        var result = _sut.CreateAlias(firstName, lastName, orderNumber);
        // Assert
        result.Should().Be(expectedAlias);
    }
}
