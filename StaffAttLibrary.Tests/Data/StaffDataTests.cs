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
using StaffAttLibrary.Enums;
using System.Net.Mail;

namespace StaffAttLibrary.Tests.Data;
public class StaffDataTests
{
    private readonly StaffData _sut;
    private readonly Mock<ISqlDataAccess> _dbMock = new();
    private readonly Mock<IStaffDataProcessor> _staffDataProcessorMock = new();
    private readonly Mock<IConnectionStringData> _connectionStringMock = new();

    public StaffDataTests()
    {
        _sut = new StaffData(_dbMock.Object, _staffDataProcessorMock.Object, _connectionStringMock.Object);
    }

    [Fact]
    public async Task SaveStaffAsync_ShouldReturnStaffId()
    {
        // Arrange
        int departmentId = 1;
        string firstName = "John";
        string lastName = "Doe";
        string emailAddress = "john.doe@johndoe.com";
        int addressId = 1;
        int aliasId = 1;
        int expectedId = 1;
        _dbMock.Setup(db => db.SaveDataGetIdAsync("spStaffs_Insert",
                                                  It.IsAny<object>(),
                                                  It.IsAny<string>()))
            .ReturnsAsync(expectedId);
        // Act
        var result = await _sut.SaveStaffAsync(departmentId, firstName, lastName, emailAddress, addressId, aliasId);
        // Assert
        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task SaveAddressAsync_ShouldReturnAddressId()
    {
        // Arrange
        AddressModel address = new AddressModel
        {
            Street = "123 Main St",
            City = "Anytown",
            Zip = "12345",
            State = "CA"
        };
        int expectedId = 1;
        _dbMock.Setup(db => db.SaveDataGetIdAsync("spAddresses_Insert",
                                                  It.IsAny<object>(),
                                                  It.IsAny<string>()))
            .ReturnsAsync(expectedId);
        // Act
        var result = await _sut.SaveAddressAsync(address);
        // Assert
        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task UpdateStaffAsync_ShouldCallSaveDataAsync()
    {
        // Arrange
        string firstName = "John";
        string lastName = "Doe";
        int staffId = 1;
        _dbMock.Setup(db => db.SaveDataAsync("spStaffs_Update",
                                              It.IsAny<object>(),
                                              It.IsAny<string>()));
        // Act
        await _sut.UpdateStaffAsync(firstName, lastName, staffId);
        // Assert
        _dbMock.Verify(db => db.SaveDataAsync("spStaffs_Update",
                                               It.IsAny<object>(),
                                               It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAliasAsync_ShouldCallSaveDataAsync()
    {
        // Arrange
        string pIN = "1234";
        int aliasId = 1;
        _dbMock.Setup(db => db.SaveDataAsync("spAliases_Update",
                                              It.IsAny<object>(),
                                              It.IsAny<string>()));
        // Act
        await _sut.UpdateAliasAsync(pIN, aliasId);
        // Assert
        _dbMock.Verify(db => db.SaveDataAsync("spAliases_Update",
                                               It.IsAny<object>(),
                                               It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAddressAsync_ShouldCallSaveDataAsync()
    {
        // Arrange
        AddressModel address = new AddressModel
        {
            Street = "123 Main St",
            City = "Anytown",
            Zip = "12345",
            State = "CA"
        };
        int addressId = 1;
        _dbMock.Setup(db => db.SaveDataAsync("spAddresses_Update",
                                              It.IsAny<object>(),
                                              It.IsAny<string>()));
        // Act
        await _sut.UpdateAddressAsync(address, addressId);
        // Assert
        _dbMock.Verify(db => db.SaveDataAsync("spAddresses_Update",
                                               It.IsAny<object>(),
                                               It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetAllBasicStaffByDepartmentAndApprovedAsync_ShouldReturnApprovedStaffList()
    {
        // Arrange
        int departmentId = 1;
        ApprovedType approvedType = ApprovedType.Approved;
        var expectedStaffList = new List<StaffBasicModel>
        {
            new StaffBasicModel
            {
                Id = departmentId,
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "john.doe@johndoe.com",
                Alias = "JDO1",
                IsApproved = true,
                DepartmentId = 1,
                Title = "IT"
            }
        };
        _dbMock.Setup(db => db.LoadDataAsync<StaffBasicModel, dynamic>(
                "spStaffs_GetAllBasicByDepartmentAndApproved",
                It.IsAny<object>(),
                It.IsAny<string>()))
            .ReturnsAsync(expectedStaffList);
        // Act
        var result = await _sut.GetAllBasicStaffByDepartmentAndApprovedAsync(departmentId, approvedType);
        // Assert
        result.Should().BeEquivalentTo(expectedStaffList);
    }

    [Fact]
    public async Task GetAllBasicStaffByDepartmentAsync_ShouldReturnStaffList()
    {
        // Arrange
        int departmentId = 1;
        var expectedStaffList = new List<StaffBasicModel>
        {
            new StaffBasicModel
            {
                Id = departmentId,
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "john.doe@johndoe.com",
                Alias = "JDO1",
                IsApproved = false,
                DepartmentId = 1,
                Title = "IT"
            }
        };
        _dbMock.Setup(db => db.LoadDataAsync<StaffBasicModel, dynamic>(
                "spStaffs_GetAllBasicByDepartment",
                It.IsAny<object>(),
                It.IsAny<string>()))
            .ReturnsAsync(expectedStaffList);
        // Act
        var result = await _sut.GetAllBasicStaffByDepartmentAsync(departmentId);
        // Assert
        result.Should().BeEquivalentTo(expectedStaffList);
    }

    [Fact]
    public async Task GetPhoneNumbersByStaffIdAsync_ShouldReturnPhoneNumbersList()
    {
        // Arrange
        int staffId = 1;
        var expectedPhoneNumbers = new List<PhoneNumberModel>
        {
            new PhoneNumberModel { PhoneNumber = "1234567890" }
        };
        _dbMock.Setup(db => db.LoadDataAsync<PhoneNumberModel, dynamic>(
                "spPhoneNumbers_GetByStaffId",
                It.IsAny<object>(),
                It.IsAny<string>()))
            .ReturnsAsync(expectedPhoneNumbers);
        // Act
        var result = await _sut.GetPhoneNumbersByStaffIdAsync(staffId);
        // Assert
        result.Should().BeEquivalentTo(expectedPhoneNumbers);
    }

    [Fact]
    public async Task GetAddressByEmailAsync_ShouldReturnAddress()
    {
        // Arrange
        string emailAddress = "john.doe@johndoe.com";
        var expectedAddress = new AddressModel
        {
            Street = "123 Main St",
            City = "Anytown",
            Zip = "12345",
            State = "CA"
        };
        _dbMock.Setup(db => db.LoadDataAsync<AddressModel, dynamic>(
                "spAddresses_GetByEmail",
                It.IsAny<object>(),
                It.IsAny<string>()))
            .ReturnsAsync(new List<AddressModel> { expectedAddress });
        // Act
        var result = await _sut.GetAddressByEmailAsync(emailAddress);
        // Assert
        result.Should().BeEquivalentTo(expectedAddress);
    }

    [Fact]
    public async Task GetStaffByEmailAsync_ShouldReturnStaff()
    {
        // Arrange
        string emailAddress = "john.doe@johndoe.com";
        var expectedStaff = new StaffFullModel
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = emailAddress,
            Alias = "JDO1",
            IsApproved = true,
            DepartmentId = 1,
            Title = "IT"
        };
        _dbMock.Setup(db => db.LoadDataAsync<StaffFullModel, dynamic>(
                "spStaffs_GetBasicByEmail",
                It.IsAny<object>(),
                It.IsAny<string>()))
            .ReturnsAsync(new List<StaffFullModel> { expectedStaff });
        // Act
        var result = await _sut.GetStaffByEmailAsync(emailAddress);
        // Assert
        result.Should().BeEquivalentTo(expectedStaff);
    }

    [Fact]
    public async Task GetAddressByIdAsync_ShouldReturnAddress()
    {
        // Arrange
        int staffId = 1;
        var expectedAddress = new AddressModel
        {
            Street = "123 Main St",
            City = "Anytown",
            Zip = "12345",
            State = "CA"
        };
        _dbMock.Setup(db => db.LoadDataAsync<AddressModel, dynamic>(
                "spAddresses_GetById",
                It.IsAny<object>(),
                It.IsAny<string>()))
            .ReturnsAsync(new List<AddressModel> { expectedAddress });
        // Act
        var result = await _sut.GetAddressByIdAsync(staffId);
        // Assert
        result.Should().BeEquivalentTo(expectedAddress);
    }

    [Fact]
    public async Task GetStaffByIdAsync_ShouldReturnStaff()
    {
        // Arrange
        int staffId = 1;
        var expectedStaff = new StaffFullModel
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
        _dbMock.Setup(db => db.LoadDataAsync<StaffFullModel, dynamic>(
                "spStaffs_GetById",
                It.IsAny<object>(),
                It.IsAny<string>()))
            .ReturnsAsync(new List<StaffFullModel> { expectedStaff });
        // Act
        var result = await _sut.GetStaffByIdAsync(staffId);
        // Assert
        result.Should().BeEquivalentTo(expectedStaff);
    }

    [Fact]
    public async Task DeleteAliasAsync_ShouldCallSaveDataAsync()
    {
        // Arrange
        int aliasId = 1;
        _dbMock.Setup(db => db.SaveDataAsync("spAliases_Delete",
                                              It.IsAny<object>(),
                                              It.IsAny<string>()));
        // Act
        await _sut.DeleteAliasAsync(aliasId);
        // Assert
        _dbMock.Verify(db => db.SaveDataAsync("spAliases_Delete",
                                               It.IsAny<object>(),
                                               It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAddressAsync_ShouldCallSaveDataAsync()
    {
        // Arrange
        int addressId = 1;
        _dbMock.Setup(db => db.SaveDataAsync("spAddresses_Delete",
                                              It.IsAny<object>(),
                                              It.IsAny<string>()));
        // Act
        await _sut.DeleteAddressAsync(addressId);
        // Assert
        _dbMock.Verify(db => db.SaveDataAsync("spAddresses_Delete",
                                               It.IsAny<object>(),
                                               It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task DeleteStaffAsync_ShouldCallSaveDataAsync()
    {
        // Arrange
        int staffId = 1;
        _dbMock.Setup(db => db.SaveDataAsync("spStaffs_Delete",
                                              It.IsAny<object>(),
                                              It.IsAny<string>()));
        // Act
        await _sut.DeleteStaffAsync(staffId);
        // Assert
        _dbMock.Verify(db => db.SaveDataAsync("spStaffs_Delete",
                                               It.IsAny<object>(),
                                               It.IsAny<string>()), Times.Once);
    }
}
