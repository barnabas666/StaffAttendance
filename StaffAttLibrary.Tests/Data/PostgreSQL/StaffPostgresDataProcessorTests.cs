using FluentAssertions;
using Moq;
using StaffAttLibrary.Data;
using StaffAttLibrary.Data.PostgreSQL;
using StaffAttLibrary.Db.PostgreSQL;
using StaffAttLibrary.Helpers;
using StaffAttLibrary.Models;

namespace StaffAttLibrary.Tests.Data.PostgreSQL;
public class StaffPostgresDataProcessorTests
{
    private readonly StaffPostgresDataProcessor _sut;
    private readonly Mock<IPostgresDataAccess> _dbMock = new();
    private readonly Mock<IConnectionStringData> _connectionStringMock = new();

    public StaffPostgresDataProcessorTests()
    {
        _sut = new StaffPostgresDataProcessor(_dbMock.Object, _connectionStringMock.Object);
    }

    [Fact]
    public async Task SavePhoneNumberAsync_ShouldReturnPhoneNumberId()
    {
        // Arrange
        var phoneNumber = new PhoneNumberModel { PhoneNumber = "1234567890" };
        int expectedId = 1;
        string sql = await QueryHelper.LoadPostgresQueryAsync("PhoneNumbers_Insert.sql");
        _dbMock.Setup(db => db.SaveDataGetIdAsync(sql,
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
        string sql = await QueryHelper.LoadPostgresQueryAsync("StaffPhoneNumbers_Insert.sql");
        _dbMock.Setup(db => db.SaveDataAsync(sql,
                                             It.IsAny<object>(),
                                             It.IsAny<string>()));
        // Act
        await _sut.SavePhoneNumberLinkAsync(staffId, phoneNumberId);
        // Assert
        _dbMock.Verify(db => db.SaveDataAsync(sql,
                                              It.IsAny<object>(),
                                              It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task CheckPhoneNumberAsync_ShouldReturnTrue_WhenPhoneNumberExists()
    {
        // Arrange
        var phoneNumber = new PhoneNumberModel { PhoneNumber = "123456789" };
        string sql = await QueryHelper.LoadPostgresQueryAsync("PhoneNumbers_Check.sql");
        _dbMock.Setup(db => db.LoadDataAsync<bool, dynamic>(
                sql,
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
        string sql = await QueryHelper.LoadPostgresQueryAsync("PhoneNumbers_GetIdByPhoneNumber.sql");
        _dbMock.Setup(db => db.LoadDataAsync<int, dynamic>(
                sql,
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
        string sql = await QueryHelper.LoadPostgresQueryAsync("PhoneNumbers_Delete.sql");
        _dbMock.Setup(db => db.SaveDataAsync(sql,
                                             It.IsAny<object>(),
                                             It.IsAny<string>()));
        // Act
        await _sut.DeletePhoneNumberAsync(phoneNumberId);
        // Assert
        _dbMock.Verify(db => db.SaveDataAsync(sql,
                                              It.IsAny<object>(),
                                              It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task DeletePhoneNumberLinkAsync_ShouldCallSaveDataAsync()
    {
        // Arrange
        int staffId = 1;
        int phoneNumberId = 2;
        string sql = await QueryHelper.LoadPostgresQueryAsync("StaffPhoneNumbers_Delete.sql");
        _dbMock.Setup(db => db.SaveDataAsync(sql,
                                             It.IsAny<object>(),
                                             It.IsAny<string>()));
        // Act
        await _sut.DeletePhoneNumberLinkAsync(staffId, phoneNumberId);
        // Assert
        _dbMock.Verify(db => db.SaveDataAsync(sql,
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
        string sql = await QueryHelper.LoadPostgresQueryAsync("StaffPhoneNumbers_GetByPhoneNumber.sql");
        _dbMock.Setup(db => db.LoadDataAsync<StaffPhoneNumberModel, dynamic>(
                sql,
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
        string sql = await QueryHelper.LoadPostgresQueryAsync("Aliases_Check.sql");
        _dbMock.Setup(db => db.LoadDataAsync<bool, dynamic>(
                sql,
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
        string sql = await QueryHelper.LoadPostgresQueryAsync("Aliases_Check.sql");
        _dbMock.Setup(db => db.LoadDataAsync<bool, dynamic>(
                sql,
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
        string sql = await QueryHelper.LoadPostgresQueryAsync("Aliases_Insert.sql");
        _dbMock.Setup(db => db.SaveDataGetIdAsync(sql,
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


