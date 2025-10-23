using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using StaffAttApi.Controllers;
using StaffAttLibrary.Data;
using StaffAttLibrary.Models;
using StaffAttShared.DTOs;
using StaffAttShared.Enums;

namespace StaffAttApi.Tests.Controllers;
public class StaffControllerTests
{
    private readonly StaffController _sut;
    private readonly Mock<IStaffService> _staffServiceMock = new();
    private readonly Mock<ILogger<StaffController>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    public StaffControllerTests()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"Caching:ShortDuration", "10"},
            {"Caching:MediumDuration", "30"},
            {"Caching:LongDuration", "120"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _sut = new StaffController(
            _staffServiceMock.Object,
            _loggerMock.Object,
            _mapperMock.Object,
            configuration);
    }

    [Fact]
    public async Task GetAllDepartments_ReturnsOk_WithMappedDtos_WhenDataExists()
    {
        // Arrange
        var departments = new List<DepartmentModel>
        {
        new DepartmentModel { Id = 1, Title = "HR", Description = "Human Resources" }
        };
        var departmentDtos = new List<DepartmentDto>
        {
        new DepartmentDto { Id = 1, Title = "HR", Description = "Human Resources" }
        };

        _staffServiceMock
            .Setup(s => s.GetAllDepartmentsAsync())
            .ReturnsAsync(departments);

        _mapperMock
            .Setup(m => m.Map<List<DepartmentDto>>(departments))
            .Returns(departmentDtos);

        // Act
        var result = await _sut.GetAllDepartments();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(departmentDtos);
        _staffServiceMock.Verify(s => s.GetAllDepartmentsAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<List<DepartmentDto>>(departments), Times.Once);
    }

    [Fact]
    public async Task GetAllDepartments_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        _staffServiceMock
            .Setup(s => s.GetAllDepartmentsAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _sut.GetAllDepartments();

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
        _staffServiceMock.Verify(s => s.GetAllDepartmentsAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateStaff_ReturnsOk_WithStaffId_WhenSuccessful()
    {
        // Arrange
        var request = new CreateStaffRequest
        {
            DepartmentId = 1,
            Address = new AddressDto { Street = "Main", City = "Zlin", Zip = "76001", State = "CZ" },
            PIN = "1234",
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john.doe@example.com",
            PhoneNumbers = new List<PhoneNumberDto>
        {
            new PhoneNumberDto { PhoneNumber = "+420123456789" }
        }
        };

        var addressModel = new AddressModel { Street = "Main", City = "Zlin", Zip = "76001", State = "CZ" };
        var phoneModels = new List<PhoneNumberModel> { new PhoneNumberModel { PhoneNumber = "+420123456789" } };

        _mapperMock.Setup(m => m.Map<AddressModel>(request.Address)).Returns(addressModel);
        _mapperMock.Setup(m => m.Map<List<PhoneNumberModel>>(request.PhoneNumbers)).Returns(phoneModels);

        _staffServiceMock
            .Setup(s => s.CreateStaffAsync(
                request.DepartmentId,
                addressModel,
                request.PIN,
                request.FirstName,
                request.LastName,
                request.EmailAddress,
                phoneModels))
            .ReturnsAsync(123);

        // Act
        var result = await _sut.CreateStaff(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { StaffId = 123 });
        _staffServiceMock.Verify(s => s.CreateStaffAsync(
            request.DepartmentId,
            addressModel,
            request.PIN,
            request.FirstName,
            request.LastName,
            request.EmailAddress,
            phoneModels), Times.Once);
    }

    [Fact]
    public async Task CreateStaff_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var request = new CreateStaffRequest
        {
            DepartmentId = 1,
            Address = new AddressDto(),
            PIN = "9999",
            FirstName = "Jane",
            LastName = "Smith",
            EmailAddress = "jane.smith@example.com",
            PhoneNumbers = new List<PhoneNumberDto>()
        };

        _mapperMock.Setup(m => m.Map<AddressModel>(It.IsAny<AddressDto>())).Returns(new AddressModel());
        _mapperMock.Setup(m => m.Map<List<PhoneNumberModel>>(It.IsAny<List<PhoneNumberDto>>())).Returns(new List<PhoneNumberModel>());

        _staffServiceMock
            .Setup(s => s.CreateStaffAsync(
                It.IsAny<int>(),
                It.IsAny<AddressModel>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<List<PhoneNumberModel>>()))
            .ThrowsAsync(new Exception("Insert failed"));

        // Act
        var result = await _sut.CreateStaff(request);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
        _staffServiceMock.Verify(s => s.CreateStaffAsync(
            It.IsAny<int>(),
            It.IsAny<AddressModel>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<List<PhoneNumberModel>>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStaff_ReturnsOk_WhenUpdateSucceeds()
    {
        // Arrange
        var request = new UpdateStaffRequest
        {
            Address = new AddressDto { Street = "Main", City = "Zlin", Zip = "76001", State = "CZ" },
            PIN = "5678",
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john.doe@example.com",
            PhoneNumbers = new List<PhoneNumberDto>
        {
            new PhoneNumberDto { PhoneNumber = "+420123456789" }
        }
        };

        var addressModel = new AddressModel { Street = "Main", City = "Zlin", Zip = "76001", State = "CZ" };
        var phoneModels = new List<PhoneNumberModel> { new PhoneNumberModel { PhoneNumber = "+420123456789" } };

        _mapperMock.Setup(m => m.Map<AddressModel>(request.Address)).Returns(addressModel);
        _mapperMock.Setup(m => m.Map<List<PhoneNumberModel>>(request.PhoneNumbers)).Returns(phoneModels);

        _staffServiceMock
            .Setup(s => s.UpdateStaffAsync(
                addressModel,
                request.PIN,
                request.FirstName,
                request.LastName,
                request.EmailAddress,
                phoneModels))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateStaff(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { request.EmailAddress });
        _staffServiceMock.Verify(s => s.UpdateStaffAsync(
            addressModel,
            request.PIN,
            request.FirstName,
            request.LastName,
            request.EmailAddress,
            phoneModels), Times.Once);
    }

    [Fact]
    public async Task UpdateStaff_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var request = new UpdateStaffRequest
        {
            Address = new AddressDto(),
            PIN = "9999",
            FirstName = "Jane",
            LastName = "Smith",
            EmailAddress = "jane.smith@example.com",
            PhoneNumbers = new List<PhoneNumberDto>()
        };

        _mapperMock.Setup(m => m.Map<AddressModel>(It.IsAny<AddressDto>())).Returns(new AddressModel());
        _mapperMock.Setup(m => m.Map<List<PhoneNumberModel>>(It.IsAny<List<PhoneNumberDto>>())).Returns(new List<PhoneNumberModel>());

        _staffServiceMock
            .Setup(s => s.UpdateStaffAsync(
                It.IsAny<AddressModel>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<List<PhoneNumberModel>>()))
            .ThrowsAsync(new Exception("Update failed"));

        // Act
        var result = await _sut.UpdateStaff(request);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
        _staffServiceMock.Verify(s => s.UpdateStaffAsync(
            It.IsAny<AddressModel>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<List<PhoneNumberModel>>()), Times.Once);
    }

    [Fact]
    public async Task GetAllBasicStaffFiltered_ReturnsOk_WithMappedDtos_WhenDataExists()
    {
        // Arrange
        int departmentId = 5;
        var approvedType = ApprovedType.Approved;

        var staffModels = new List<StaffBasicModel>
        {
        new StaffBasicModel { Id = 1, FirstName = "John", LastName = "Doe", EmailAddress = "john@doe.com" }
        };

        var staffDtos = new List<StaffBasicDto>
        {
        new StaffBasicDto { Id = 1, FirstName = "John", LastName = "Doe", EmailAddress = "john@doe.com" }
        };

        _staffServiceMock
            .Setup(s => s.GetAllBasicStaffFilteredAsync(departmentId, approvedType))
            .ReturnsAsync(staffModels);

        _mapperMock
            .Setup(m => m.Map<List<StaffBasicDto>>(staffModels))
            .Returns(staffDtos);

        // Act
        var result = await _sut.GetAllBasicStaffFiltered(departmentId, approvedType);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(staffDtos);
        _staffServiceMock.Verify(s => s.GetAllBasicStaffFilteredAsync(departmentId, approvedType), Times.Once);
    }

    [Fact]
    public async Task GetAllBasicStaffFiltered_ReturnsNotFound_WhenNoStaffFound()
    {
        // Arrange
        int departmentId = 3;
        var approvedType = ApprovedType.NotApproved;

        _staffServiceMock
            .Setup(s => s.GetAllBasicStaffFilteredAsync(departmentId, approvedType))
            .ReturnsAsync((List<StaffBasicModel>?)null);

        // Act
        var result = await _sut.GetAllBasicStaffFiltered(departmentId, approvedType);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
        _staffServiceMock.Verify(s => s.GetAllBasicStaffFilteredAsync(departmentId, approvedType), Times.Once);
    }

    [Fact]
    public async Task GetAllBasicStaffFiltered_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        int departmentId = 2;
        var approvedType = ApprovedType.All;

        _staffServiceMock
            .Setup(s => s.GetAllBasicStaffFilteredAsync(departmentId, approvedType))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _sut.GetAllBasicStaffFiltered(departmentId, approvedType);

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
        _staffServiceMock.Verify(s => s.GetAllBasicStaffFilteredAsync(departmentId, approvedType), Times.Once);
    }

    [Fact]
    public async Task GetAllBasicStaff_ReturnsOk_WithMappedDtos_WhenDataExists()
    {
        // Arrange
        var staffModels = new List<StaffBasicModel>
        {
        new() { Id = 1, FirstName = "John", LastName = "Doe", EmailAddress = "john@doe.com" }
        };

        var staffDtos = new List<StaffBasicDto>
        {
        new() { Id = 1, FirstName = "John", LastName = "Doe", EmailAddress = "john@doe.com" }
        };

        _staffServiceMock
            .Setup(s => s.GetAllBasicStaffAsync())
            .ReturnsAsync(staffModels);

        _mapperMock
            .Setup(m => m.Map<List<StaffBasicDto>>(staffModels))
            .Returns(staffDtos);

        // Act
        var result = await _sut.GetAllBasicStaff();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(staffDtos);
        _staffServiceMock.Verify(s => s.GetAllBasicStaffAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllBasicStaff_ReturnsNotFound_WhenNoStaffFound()
    {
        // Arrange
        _staffServiceMock
            .Setup(s => s.GetAllBasicStaffAsync())
            .ReturnsAsync((List<StaffBasicModel>?)null);

        // Act
        var result = await _sut.GetAllBasicStaff();

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
        _staffServiceMock.Verify(s => s.GetAllBasicStaffAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllBasicStaff_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        _staffServiceMock
            .Setup(s => s.GetAllBasicStaffAsync())
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.GetAllBasicStaff();

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
        _staffServiceMock.Verify(s => s.GetAllBasicStaffAsync(), Times.Once);
    }

    [Fact]
    public async Task GetStaffByEmail_ReturnsOk_WithMappedDto_WhenStaffExists()
    {
        // Arrange
        var email = "jane.doe@example.com";

        var staffModel = new StaffFullModel
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Doe",
            EmailAddress = email
        };

        var staffDto = new StaffFullDto
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Doe",
            EmailAddress = email
        };

        _staffServiceMock
            .Setup(s => s.GetStaffByEmailAsync(email))
            .ReturnsAsync(staffModel);

        _mapperMock
            .Setup(m => m.Map<StaffFullDto>(staffModel))
            .Returns(staffDto);

        // Act
        var result = await _sut.GetStaffByEmail(email);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(staffDto);
        _staffServiceMock.Verify(s => s.GetStaffByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task GetStaffByEmail_ReturnsNotFound_WhenStaffNotFound()
    {
        // Arrange
        var email = "missing@example.com";

        _staffServiceMock
            .Setup(s => s.GetStaffByEmailAsync(email))
            .ReturnsAsync((StaffFullModel?)null);

        // Act
        var result = await _sut.GetStaffByEmail(email);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
        _staffServiceMock.Verify(s => s.GetStaffByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task GetStaffByEmail_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var email = "error@example.com";

        _staffServiceMock
            .Setup(s => s.GetStaffByEmailAsync(email))
            .ThrowsAsync(new Exception("Database failure"));

        // Act
        var result = await _sut.GetStaffByEmail(email);

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
        _staffServiceMock.Verify(s => s.GetStaffByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task GetStaffById_ReturnsOk_WithMappedDto_WhenStaffExists()
    {
        // Arrange
        int id = 1;
        var staffModel = new StaffFullModel { Id = id, FirstName = "John", LastName = "Doe" };
        var staffDto = new StaffFullDto { Id = id, FirstName = "John", LastName = "Doe" };

        _staffServiceMock
            .Setup(s => s.GetStaffByIdAsync(id))
            .ReturnsAsync(staffModel);

        _mapperMock
            .Setup(m => m.Map<StaffFullDto>(staffModel))
            .Returns(staffDto);

        // Act
        var result = await _sut.GetStaffById(id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(staffDto);
        _staffServiceMock.Verify(s => s.GetStaffByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetStaffById_ReturnsNotFound_WhenStaffNotFound()
    {
        // Arrange
        int id = 999;
        _staffServiceMock
            .Setup(s => s.GetStaffByIdAsync(id))
            .ReturnsAsync((StaffFullModel?)null);

        // Act
        var result = await _sut.GetStaffById(id);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
        _staffServiceMock.Verify(s => s.GetStaffByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetStaffById_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        int id = 2;
        _staffServiceMock
            .Setup(s => s.GetStaffByIdAsync(id))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _sut.GetStaffById(id);

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
        _staffServiceMock.Verify(s => s.GetStaffByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetBasicStaffById_ReturnsOk_WithMappedDto_WhenStaffExists()
    {
        // Arrange
        int id = 3;
        var staffModel = new StaffBasicModel
        {
            Id = id,
            FirstName = "John",
            LastName = "Smith",
            EmailAddress = "john.smith@example.com"
        };
        var staffDto = new StaffBasicDto
        {
            Id = id,
            FirstName = "John",
            LastName = "Smith",
            EmailAddress = "john.smith@example.com"
        };

        _staffServiceMock
            .Setup(s => s.GetBasicStaffByIdAsync(id))
            .ReturnsAsync(staffModel);

        _mapperMock
            .Setup(m => m.Map<StaffBasicDto>(staffModel))
            .Returns(staffDto);

        // Act
        var result = await _sut.GetBasicStaffById(id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(staffDto);
        _staffServiceMock.Verify(s => s.GetBasicStaffByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetBasicStaffById_ReturnsNotFound_WhenStaffNotFound()
    {
        // Arrange
        int id = 12;
        _staffServiceMock
            .Setup(s => s.GetBasicStaffByIdAsync(id))
            .ReturnsAsync((StaffBasicModel?)null);

        // Act
        var result = await _sut.GetBasicStaffById(id);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
        _staffServiceMock.Verify(s => s.GetBasicStaffByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetBasicStaffById_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        int id = 8;
        _staffServiceMock
            .Setup(s => s.GetBasicStaffByIdAsync(id))
            .ThrowsAsync(new Exception("Database failure"));

        // Act
        var result = await _sut.GetBasicStaffById(id);

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
        _staffServiceMock.Verify(s => s.GetBasicStaffByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetBasicStaffByAliasId_ReturnsOk_WithMappedDto_WhenStaffExists()
    {
        // Arrange
        int aliasId = 456;
        var staffModel = new StaffBasicModel
        {
            Id = 1,
            FirstName = "Alice",
            LastName = "Brown",
            EmailAddress = "alice.brown@example.com",
            Alias = "AB"
        };
        var staffDto = new StaffBasicDto
        {
            Id = 1,
            FirstName = "Alice",
            LastName = "Brown",
            EmailAddress = "alice.brown@example.com",
            Alias = "AB"
        };

        _staffServiceMock
            .Setup(s => s.GetBasicStaffByAliasIdAsync(aliasId))
            .ReturnsAsync(staffModel);

        _mapperMock
            .Setup(m => m.Map<StaffBasicDto>(staffModel))
            .Returns(staffDto);

        // Act
        var result = await _sut.GetBasicStaffByAliasId(aliasId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(staffDto);
        _staffServiceMock.Verify(s => s.GetBasicStaffByAliasIdAsync(aliasId), Times.Once);
    }

    [Fact]
    public async Task GetBasicStaffByAliasId_ReturnsNotFound_WhenStaffNotFound()
    {
        // Arrange
        int aliasId = 999;
        _staffServiceMock
            .Setup(s => s.GetBasicStaffByAliasIdAsync(aliasId))
            .ReturnsAsync((StaffBasicModel?)null);

        // Act
        var result = await _sut.GetBasicStaffByAliasId(aliasId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
        _staffServiceMock.Verify(s => s.GetBasicStaffByAliasIdAsync(aliasId), Times.Once);
    }

    [Fact]
    public async Task GetBasicStaffByAliasId_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        int aliasId = 101;
        _staffServiceMock
            .Setup(s => s.GetBasicStaffByAliasIdAsync(aliasId))
            .ThrowsAsync(new Exception("Database failure"));

        // Act
        var result = await _sut.GetBasicStaffByAliasId(aliasId);

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
        _staffServiceMock.Verify(s => s.GetBasicStaffByAliasIdAsync(aliasId), Times.Once);
    }

    [Fact]
    public async Task GetStaffEmailById_ReturnsOk_WithEmail_WhenFound()
    {
        // Arrange
        int id = 12;
        string email = "bob.jones@example.com";

        _staffServiceMock
            .Setup(s => s.GetStaffEmailByIdAsync(id))
            .ReturnsAsync(email);

        // Act
        var result = await _sut.GetStaffEmailById(id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { EmailAddress = email });
        _staffServiceMock.Verify(s => s.GetStaffEmailByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetStaffEmailById_ReturnsNotFound_WhenEmailIsNullOrEmpty()
    {
        // Arrange
        int id = 33;
        _staffServiceMock
            .Setup(s => s.GetStaffEmailByIdAsync(id))
            .ReturnsAsync((string?)null);

        // Act
        var result = await _sut.GetStaffEmailById(id);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
        _staffServiceMock.Verify(s => s.GetStaffEmailByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetStaffEmailById_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        int id = 44;
        _staffServiceMock
            .Setup(s => s.GetStaffEmailByIdAsync(id))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _sut.GetStaffEmailById(id);

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
        _staffServiceMock.Verify(s => s.GetStaffEmailByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task CheckStaffByEmail_ReturnsOk_WithResult_WhenServiceSucceeds()
    {
        // Arrange
        string email = "someone@example.com";
        bool exists = true;

        _staffServiceMock
            .Setup(s => s.CheckStaffByEmailAsync(email))
            .ReturnsAsync(exists);

        // Act
        var result = await _sut.CheckStaffByEmail(email);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(exists);
        _staffServiceMock.Verify(s => s.CheckStaffByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task CheckStaffByEmail_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        string email = "error@example.com";

        _staffServiceMock
            .Setup(s => s.CheckStaffByEmailAsync(email))
            .ThrowsAsync(new Exception("Database failure"));

        // Act
        var result = await _sut.CheckStaffByEmail(email);

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
        _staffServiceMock.Verify(s => s.CheckStaffByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task UpdateStaffByAdmin_ReturnsOk_WhenUpdateSucceeds()
    {
        // Arrange
        var request = new UpdateStaffByAdminRequest
        {
            Id = 10,
            DepartmentId = 5,
            IsApproved = true
        };

        _staffServiceMock
            .Setup(s => s.UpdateStaffByAdminAsync(request.Id, request.DepartmentId, request.IsApproved))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateStaffByAdmin(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { request.Id });
        _staffServiceMock.Verify(s => s.UpdateStaffByAdminAsync(request.Id, request.DepartmentId, request.IsApproved), Times.Once);
    }

    [Fact]
    public async Task UpdateStaffByAdmin_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var request = new UpdateStaffByAdminRequest
        {
            Id = 11,
            DepartmentId = 2,
            IsApproved = false
        };

        _staffServiceMock
            .Setup(s => s.UpdateStaffByAdminAsync(request.Id, request.DepartmentId, request.IsApproved))
            .ThrowsAsync(new Exception("Unexpected failure"));

        // Act
        var result = await _sut.UpdateStaffByAdmin(request);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
        _staffServiceMock.Verify(s => s.UpdateStaffByAdminAsync(request.Id, request.DepartmentId, request.IsApproved), Times.Once);
    }

    [Fact]
    public async Task DeleteStaff_ReturnsOk_WhenDeleteSucceeds()
    {
        // Arrange
        int staffId = 123;

        _staffServiceMock
            .Setup(s => s.DeleteStaffAsync(staffId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteStaff(staffId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { StaffId = staffId });
        _staffServiceMock.Verify(s => s.DeleteStaffAsync(staffId), Times.Once);
    }

    [Fact]
    public async Task DeleteStaff_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        int staffId = 500;

        _staffServiceMock
            .Setup(s => s.DeleteStaffAsync(staffId))
            .ThrowsAsync(new Exception("Failed to delete"));

        // Act
        var result = await _sut.DeleteStaff(staffId);

        // Assert
        result.Should().BeOfType<BadRequestResult>();

        _staffServiceMock.Verify(s => s.DeleteStaffAsync(staffId), Times.Once);
    }
}
