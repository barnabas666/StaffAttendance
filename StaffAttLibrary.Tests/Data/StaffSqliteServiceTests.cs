using FluentAssertions;
using Moq;
using StaffAttLibrary.Data;
using StaffAttLibrary.Db;
using StaffAttLibrary.Enums;
using StaffAttLibrary.Helpers;
using StaffAttLibrary.Models;

namespace StaffAttLibrary.Tests.Data;
public class StaffSqliteServiceTests
{
    private readonly StaffSqliteService _sut;
    private readonly Mock<ISqliteDataAccess> _dbMock = new();
    private readonly Mock<IStaffData> _staffDataMock = new();
    private readonly Mock<ICheckInData> _checkInDataMock = new();
    private readonly Mock<IConnectionStringData> _connectionStringMock = new();

    public StaffSqliteServiceTests()
    {
        _sut = new StaffSqliteService(_dbMock.Object, _staffDataMock.Object, _checkInDataMock.Object, _connectionStringMock.Object);
    }

    [Fact]
    public async Task GetAllDepartmentsAsync_ShouldReturnListOfDepartments()
    {
        // Arrange
        var expectedDepartments = new List<DepartmentModel>
        {
            new DepartmentModel { Id = 1, Title = "IT", Description = "IT department." },
            new DepartmentModel { Id = 2, Title = "HR", Description = "Human resources department." }
        };
        string sql = await SqliteQueryHelper.LoadQueryAsync("Departments_GetAll.sql");
        _dbMock.Setup(db => db.LoadDataAsync<DepartmentModel, dynamic>(
                sql, It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(expectedDepartments);
        // Act
        var result = await _sut.GetAllDepartmentsAsync();
        // Assert
        result.Should().BeEquivalentTo(expectedDepartments);
    }

    [Fact]
    public async Task AliasVerificationAsync_ShouldReturnAlias_WhenAliasExists()
    {
        // Arrange
        string alias = "JDO1";
        string pIN = "1234";
        var expectedAlias = new AliasModel { Id = 1, Alias = alias };
        string sql = await SqliteQueryHelper.LoadQueryAsync("Aliases_GetByAliasAndPIN.sql");
        _dbMock.Setup(db => db.LoadDataAsync<AliasModel, dynamic>(
                sql, It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(new List<AliasModel> { expectedAlias });
        // Act
        var result = await _sut.AliasVerificationAsync(alias, pIN);
        // Assert
        result.Should().BeEquivalentTo(expectedAlias);
    }

    // Maybe this belongs to integration tests?
    [Fact]
    public async Task GetAllBasicStaffFilteredAsync_ShouldCallGetAllBasicStaffByDepartmentAsync_WhenApprovedTypeIsAll()
    {
        // Arrange
        int departmentId = 1;
        ApprovedType approvedType = ApprovedType.All;
        _staffDataMock.Setup(data => data.GetAllBasicStaffByDepartmentAsync(departmentId))
            .ReturnsAsync(new List<StaffBasicModel>());
        // Act
        await _sut.GetAllBasicStaffFilteredAsync(departmentId, approvedType);
        // Assert
        _staffDataMock.Verify(data => data.GetAllBasicStaffByDepartmentAsync(departmentId), Times.Once);
    }

    // Maybe this belongs to integration tests?
    [Fact]
    public async Task GetAllBasicStaffFilteredAsync_ShouldCallGetAllBasicStaffByDepartmentAndApprovedAsync_WhenApprovedTypeIsNotAll()
    {
        // Arrange
        int departmentId = 1;
        ApprovedType approvedType = ApprovedType.Approved;
        _staffDataMock.Setup(data => data.GetAllBasicStaffByDepartmentAndApprovedAsync(departmentId, approvedType))
            .ReturnsAsync(new List<StaffBasicModel>());
        // Act
        await _sut.GetAllBasicStaffFilteredAsync(departmentId, approvedType);
        // Assert
        _staffDataMock.Verify(data => data.GetAllBasicStaffByDepartmentAndApprovedAsync(departmentId, approvedType), Times.Once);
    }

    [Fact]
    public async Task GetAllBasicStaffAsync_ShouldReturnListOfStaffBasicModel()
    {
        // Arrange
        var expectedStaffList = new List<StaffBasicModel>
        {
            new StaffBasicModel
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "john.doe@johndoe.com",
                Alias = "JDO1",
                IsApproved = true,
                DepartmentId = 1,
                Title = "IT"
            }
        };
        string sql = await SqliteQueryHelper.LoadQueryAsync("Staffs_GetAllBasic.sql");
        _dbMock.Setup(db => db.LoadDataAsync<StaffBasicModel, dynamic>(
                sql, It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(expectedStaffList);
        // Act
        var result = await _sut.GetAllBasicStaffAsync();
        // Assert
        result.Should().BeEquivalentTo(expectedStaffList);
    }

    [Fact]
    public async Task GetBasicStaffByIdAsync_ShouldReturnStaffBasicModel()
    {
        // Arrange
        int staffId = 1;
        var expectedStaff = new StaffBasicModel
        {
            Id = staffId,
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john.doe@johndoe.com",
            Alias = "JDO1",
            IsApproved = true,
            DepartmentId = 1,
            Title = "IT"
        };
        string sql = await SqliteQueryHelper.LoadQueryAsync("Staffs_GetBasicById.sql");
        _dbMock.Setup(db => db.LoadDataAsync<StaffBasicModel, dynamic>(
                sql, It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(new List<StaffBasicModel> { expectedStaff });
        // Act
        var result = await _sut.GetBasicStaffByIdAsync(staffId);
        // Assert
        result.Should().BeEquivalentTo(expectedStaff);
    }

    [Fact]
    public async Task GetBasicStaffByAliasIdAsync_ShouldReturnStaffBasicModel()
    {
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
        string sql = await SqliteQueryHelper.LoadQueryAsync("Staffs_GetBasicByAliasId.sql");
        _dbMock.Setup(db => db.LoadDataAsync<StaffBasicModel, dynamic>(
                sql, It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(new List<StaffBasicModel> { expectedStaff });
        // Act
        var result = await _sut.GetBasicStaffByAliasIdAsync(aliasId);
        // Assert
        result.Should().BeEquivalentTo(expectedStaff);
    }

    [Fact]
    public async Task GetStaffEmailByIdAsync_ShouldReturnEmailAddress()
    {
        // Arrange
        int staffId = 1;
        string expectedEmail = "john.doe@johndoe.com";
        string sql = await SqliteQueryHelper.LoadQueryAsync("Staffs_GetEmailById.sql");
        _dbMock.Setup(db => db.LoadDataAsync<string, dynamic>(
                sql, It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(new List<string> { expectedEmail });
        // Act
        var result = await _sut.GetStaffEmailByIdAsync(staffId);
        // Assert
        result.Should().Be(expectedEmail);
    }

    [Fact]
    public async Task CheckStaffByEmailAsync_ShouldReturnTrue_WhenStaffExists()
    {
        // Arrange
        string emailAddress = "john.doe@johndoe.com";
        bool expectedResult = true;
        string sql = await SqliteQueryHelper.LoadQueryAsync("Staffs_CheckByEmail.sql");
        _dbMock.Setup(db => db.LoadDataAsync<bool, dynamic>(
                sql, It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(new List<bool> { expectedResult });
        // Act
        var result = await _sut.CheckStaffByEmailAsync(emailAddress);
        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateStaffByAdminAsync_ShouldCallSaveDataAsync()
    {
        int staffId = 1;
        int departmentId = 1;
        bool isApproved = true;
        string sql = await SqliteQueryHelper.LoadQueryAsync("Staffs_UpdateByAdmin.sql");
        _dbMock.Setup(db => db.SaveDataAsync(sql,
                It.IsAny<object>(), It.IsAny<string>()));
        // Act
        await _sut.UpdateStaffByAdminAsync(staffId, departmentId, isApproved);
        // Assert
        _dbMock.Verify(db => db.SaveDataAsync(sql,
                It.IsAny<object>(), It.IsAny<string>()), Times.Once);
    }
}