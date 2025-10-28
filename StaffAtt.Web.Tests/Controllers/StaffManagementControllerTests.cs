using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using StaffAtt.Web.Controllers;
using StaffAtt.Web.Helpers;
using StaffAtt.Web.Models;
using StaffAttShared.DTOs;
using StaffAttShared.Enums;

namespace StaffAtt.Web.Tests.Controllers;

public class StaffManagementControllerTests
{
    private readonly StaffManagementController _sut;
    private readonly Mock<IApiClient> _apiClientMock = new();
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IDepartmentSelectListService> _departmentSelectListServiceMock = new();

    public StaffManagementControllerTests()
    {
        _sut = new StaffManagementController(
            _apiClientMock.Object,
            _userServiceMock.Object,
            _mapperMock.Object,
            _departmentSelectListServiceMock.Object
        );
    }

    [Fact]
    public async Task List_ShouldReturnListViewWithStaffManagementListViewModel()
    {
        // Arrange
        string expectedViewName = "List";

        // Mock API result
        var staffDtos = new List<StaffBasicDto>
        {
            new() { Id = 1, FirstName = "John", LastName = "Doe", DepartmentId = 1, Title = "IT" },
            new() { Id = 2, FirstName = "Jane", LastName = "Smith", DepartmentId = 2, Title = "HR" }
        };
        _apiClientMock
            .Setup(api => api.GetAsync<List<StaffBasicDto>>("staff/basic"))
            .ReturnsAsync(Result<List<StaffBasicDto>>.Success(staffDtos));

        // Mock mapping
        var staffViewModels = new List<StaffBasicViewModel>();
        _mapperMock.Setup(m => m.Map<List<StaffBasicViewModel>>(staffDtos))
            .Returns(staffViewModels);

        // Mock departments
        var departments = new List<DepartmentDto>
        {
            new() { Id = 1, Title = "IT" },
            new() { Id = 2, Title = "HR" }
        };
        var selectList = new SelectList(departments, nameof(DepartmentDto.Id), nameof(DepartmentDto.Title));
        _departmentSelectListServiceMock.Setup(x => x.GetDepartmentSelectListAsync("All"))
            .ReturnsAsync(selectList);

        // Act
        var result = await _sut.List();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        var model = viewResult.Model.Should().BeAssignableTo<StaffManagementListViewModel>().Subject;
        model.DepartmentItems.Should().BeEquivalentTo(selectList);
    }

    [Fact]
    public async Task List_ShouldReturnErrorView_WhenApiCallFails()
    {
        // Arrange
        const string expectedViewName = "Error";
        const string errorMessage = "Failed to load staff list";

        _apiClientMock
            .Setup(api => api.GetAsync<List<StaffBasicDto>>("staff/basic"))
            .ReturnsAsync(Result<List<StaffBasicDto>>.Failure(errorMessage));

        // Act
        var result = await _sut.List();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        var model = viewResult.Model.Should().BeOfType<ErrorViewModel>().Subject;
        model.Message.Should().Be(errorMessage);

        _mapperMock.Verify(m => m.Map<List<StaffBasicViewModel>>(It.IsAny<List<StaffBasicDto>>()), Times.Never);
    }

    [Fact]
    public async Task ListPost_ShouldReturnListViewWithStaffManagementListViewModel()
    {
        // Arrange
        string expectedViewName = "List";
        var staffModel = new StaffManagementListViewModel
        {
            DepartmentId = "1",
            ApprovedRadio = ApprovedType.Approved
        };
        var filteredDtos = new List<StaffBasicDto>
        {
            new() { Id = 1, FirstName = "Alice", LastName = "Doe", DepartmentId = 1, Title = "IT" }
        };

        string expectedUrl = $"staff/basic/filter?departmentId={staffModel.DepartmentId}&approvedType={(int)staffModel.ApprovedRadio}";
        _apiClientMock
            .Setup(api => api.GetAsync<List<StaffBasicDto>>(expectedUrl))
            .ReturnsAsync(Result<List<StaffBasicDto>>.Success(filteredDtos));

        var mappedViewModels = new List<StaffBasicViewModel>();
        _mapperMock.Setup(m => m.Map<List<StaffBasicViewModel>>(filteredDtos))
            .Returns(mappedViewModels);

        var departments = new List<DepartmentDto>
        {
            new() { Id = 1, Title = "IT" },
            new() { Id = 2, Title = "HR" }
        };
        var selectList = new SelectList(departments, nameof(DepartmentDto.Id), nameof(DepartmentDto.Title));
        _departmentSelectListServiceMock.Setup(x => x.GetDepartmentSelectListAsync("All"))
            .ReturnsAsync(selectList);

        // Act
        var result = await _sut.List(staffModel);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        var model = viewResult.Model.Should().BeAssignableTo<StaffManagementListViewModel>().Subject;
        model.DepartmentItems.Should().BeEquivalentTo(selectList);
    }

    [Fact]
    public async Task ListPost_ShouldReturnErrorView_WhenApiCallFails()
    {
        // Arrange
        const string expectedViewName = "Error";
        const string errorMessage = "Filtering failed";

        var staffModel = new StaffManagementListViewModel
        {
            DepartmentId = "2",
            ApprovedRadio = ApprovedType.All
        };

        string expectedUrl =
            $"staff/basic/filter?departmentId={staffModel.DepartmentId}&approvedType={(int)staffModel.ApprovedRadio}";

        _apiClientMock
            .Setup(api => api.GetAsync<List<StaffBasicDto>>(expectedUrl))
            .ReturnsAsync(Result<List<StaffBasicDto>>.Failure(errorMessage));

        // Act
        var result = await _sut.List(staffModel);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        var model = viewResult.Model.Should().BeOfType<ErrorViewModel>().Subject;
        model.Message.Should().Be(errorMessage);

        _mapperMock.Verify(m => m.Map<List<StaffBasicViewModel>>(It.IsAny<List<StaffBasicDto>>()), Times.Never);
    }

    [Fact]
    public async Task Details_ShouldReturnDetailsViewWithStaffDetailsViewModel()
    {
        // Arrange
        int staffId = 1;
        string expectedViewName = "Details";
        var staffFullDto = new StaffFullDto { Id = staffId };

        _apiClientMock
            .Setup(api => api.GetAsync<StaffFullDto>($"staff/{staffId}"))
            .ReturnsAsync(Result<StaffFullDto>.Success(staffFullDto));

        var detailsModel = new StaffDetailsViewModel
        {
            BasicInfo = new StaffBasicViewModel { Id = staffId }
        };

        _mapperMock.Setup(m => m.Map<StaffDetailsViewModel>(staffFullDto))
            .Returns(detailsModel);

        // Act
        IActionResult result = await _sut.Details(staffId);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        var model = viewResult.Model.Should().BeOfType<StaffDetailsViewModel>().Subject;
        model.BasicInfo.Id.Should().Be(staffId);
    }

    [Fact]
    public async Task Details_ShouldReturnErrorView_WhenApiCallFails()
    {
        // Arrange
        int staffId = 5;
        const string expectedViewName = "Error";
        const string errorMessage = "Failed to load staff details";

        _apiClientMock
            .Setup(api => api.GetAsync<StaffFullDto>($"staff/{staffId}"))
            .ReturnsAsync(Result<StaffFullDto>.Failure(errorMessage));

        // Act
        var result = await _sut.Details(staffId);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        var model = viewResult.Model.Should().BeOfType<ErrorViewModel>().Subject;
        model.Message.Should().Be(errorMessage);

        _mapperMock.Verify(m => m.Map<StaffDetailsViewModel>(It.IsAny<StaffFullDto>()), Times.Never);
    }

    [Fact]
    public async Task Update_ShouldReturnUpdateViewWithStaffManagementUpdateViewModel()
    {
        // Arrange
        int staffId = 1;
        string expectedViewName = "Update";
        var staffBasicDto = new StaffBasicDto { Id = staffId };

        _apiClientMock
            .Setup(api => api.GetAsync<StaffBasicDto>($"staff/basic/{staffId}"))
            .ReturnsAsync(Result<StaffBasicDto>.Success(staffBasicDto));

        var updateModel = new StaffManagementUpdateViewModel
        {
            BasicInfo = new StaffBasicViewModel { Id = staffId }
        };

        _mapperMock.Setup(m => m.Map<StaffManagementUpdateViewModel>(staffBasicDto))
            .Returns(updateModel);

        var departments = new List<DepartmentDto>
    {
        new() { Id = 1, Title = "IT" },
        new() { Id = 2, Title = "HR" }
    };
        var selectList = new SelectList(departments, nameof(DepartmentDto.Id), nameof(DepartmentDto.Title));

        _departmentSelectListServiceMock
            .Setup(x => x.GetDepartmentSelectListAsync(string.Empty))
            .ReturnsAsync(selectList);

        // Act
        IActionResult result = await _sut.Update(staffId);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        var model = viewResult.Model.Should().BeOfType<StaffManagementUpdateViewModel>().Subject;
        model.BasicInfo.Id.Should().Be(staffId);
        model.DepartmentItems.Should().BeEquivalentTo(selectList);
    }

    [Fact]
    public async Task Update_ShouldReturnErrorView_WhenApiCallFails()
    {
        // Arrange
        int staffId = 1;
        const string expectedViewName = "Error";
        const string errorMessage = "Failed to load staff for update";

        _apiClientMock
            .Setup(api => api.GetAsync<StaffBasicDto>($"staff/basic/{staffId}"))
            .ReturnsAsync(Result<StaffBasicDto>.Failure(errorMessage));

        // Act
        IActionResult result = await _sut.Update(staffId);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        var model = viewResult.Model.Should().BeOfType<ErrorViewModel>().Subject;
        model.Message.Should().Be(errorMessage);

        _mapperMock.Verify(m => m.Map<StaffManagementUpdateViewModel>(It.IsAny<StaffBasicDto>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePost_ShouldRedirectToListAction()
    {
        // Arrange
        int staffId = 1;
        string expectedActionName = "List";
        const string expectedRouteName = "successMessage";

        var updateModel = new StaffManagementUpdateViewModel
        {
            BasicInfo = new StaffBasicViewModel
            {
                Id = staffId,
                FirstName = "John",
                LastName = "Doe",
                DepartmentId = 1,
                IsApproved = true
            }
        };
        string expectedMessage = $"{updateModel.BasicInfo.FirstName} {updateModel.BasicInfo.LastName} profile was successfully updated.";

        var request = new UpdateStaffByAdminRequest
        {
            Id = staffId,
            DepartmentId = 1,
            IsApproved = true
        };

        _apiClientMock
            .Setup(api => api.PutAsync("staff/admin", It.IsAny<UpdateStaffByAdminRequest>()))
            .ReturnsAsync(Result<UpdateStaffByAdminRequest>.Success(request));

        // Act
        IActionResult result = await _sut.Update(updateModel);

        // Assert
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be(expectedActionName);
        redirectResult.RouteValues[expectedRouteName].Should().Be(expectedMessage);
    }

    [Fact]
    public async Task UpdatePost_ShouldReturnErrorView_WhenApiPutFails()
    {
        // Arrange
        int staffId = 1;
        const string expectedViewName = "Error";
        const string errorMessage = "Update failed";

        var updateModel = new StaffManagementUpdateViewModel
        {
            BasicInfo = new StaffBasicViewModel
            {
                Id = staffId,
                DepartmentId = 1,
                IsApproved = true
            }
        };

        _apiClientMock
            .Setup(api => api.PutAsync("staff/admin", It.IsAny<UpdateStaffByAdminRequest>()))
            .ReturnsAsync(Result<UpdateStaffByAdminRequest>.Failure(errorMessage));

        // Act
        IActionResult result = await _sut.Update(updateModel);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        var model = viewResult.Model.Should().BeOfType<ErrorViewModel>().Subject;
        model.Message.Should().Be(errorMessage);

        _apiClientMock.Verify(api => api.PutAsync("staff/admin", It.IsAny<UpdateStaffByAdminRequest>()), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldReturnDeleteViewWithStaffManagementDeleteViewModel()
    {
        // Arrange
        int staffId = 1;
        string expectedViewName = "Delete";

        var staffBasicDto = new StaffBasicDto { Id = staffId };

        _apiClientMock
            .Setup(api => api.GetAsync<StaffBasicDto>($"staff/basic/{staffId}"))
            .ReturnsAsync(Result<StaffBasicDto>.Success(staffBasicDto));

        var deleteModel = new StaffManagementDeleteViewModel
        {
            BasicInfo = new StaffBasicViewModel { Id = staffId }
        };

        _mapperMock
            .Setup(m => m.Map<StaffManagementDeleteViewModel>(staffBasicDto))
            .Returns(deleteModel);

        // Act
        IActionResult result = await _sut.Delete(staffId);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        var model = viewResult.Model.Should().BeOfType<StaffManagementDeleteViewModel>().Subject;
        model.BasicInfo.Id.Should().Be(staffId);
    }

    [Fact]
    public async Task Delete_ShouldReturnErrorView_WhenApiCallFails()
    {
        // Arrange
        int staffId = 2;
        const string expectedViewName = "Error";
        const string errorMessage = "Failed to load staff for deletion";

        _apiClientMock
            .Setup(api => api.GetAsync<StaffBasicDto>($"staff/basic/{staffId}"))
            .ReturnsAsync(Result<StaffBasicDto>.Failure(errorMessage));

        // Act
        IActionResult result = await _sut.Delete(staffId);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        var model = viewResult.Model.Should().BeOfType<ErrorViewModel>().Subject;
        model.Message.Should().Be(errorMessage);

        _mapperMock.Verify(m => m.Map<StaffManagementDeleteViewModel>(It.IsAny<StaffBasicDto>()), Times.Never);
    }

    [Fact]
    public async Task DeletePost_ShouldRedirectToListAction()
    {
        // Arrange
        int staffId = 1;
        string userEmail = "john.doe@johndoe.com";
        string expectedActionName = "List";
        const string expectedRouteName = "successMessage";

        var deleteViewModel = new StaffManagementDeleteViewModel
        {
            BasicInfo = new StaffBasicViewModel
            {
                Id = staffId,
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = userEmail
            }
        };
        string expectedMessage = $"{deleteViewModel.BasicInfo.FirstName} {deleteViewModel.BasicInfo.LastName} profile was successfully deleted.";

        _apiClientMock
            .Setup(api => api.DeleteAsync($"staff/{staffId}"))
            .ReturnsAsync(Result<bool>.Success(true));

        IdentityUser identityUser = new() { Email = userEmail };

        _userServiceMock
            .Setup(x => x.FindByEmailAsync(userEmail))
            .ReturnsAsync(identityUser);

        _userServiceMock
            .Setup(x => x.DeleteIdentityUserAsync(identityUser))
            .Returns(Task.CompletedTask);

        // Act
        IActionResult result = await _sut.Delete(deleteViewModel);

        // Assert
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be(expectedActionName);
        redirectResult.RouteValues[expectedRouteName].Should().Be(expectedMessage);
    }

    [Fact]
    public async Task DeletePost_ShouldReturnErrorView_WhenIdIsInvalid()
    {
        // Arrange
        var deleteModel = new StaffManagementDeleteViewModel
        {
            BasicInfo = new StaffBasicViewModel { Id = 0 }
        };

        // Act
        IActionResult result = await _sut.Delete(deleteModel);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be("Error");
        var model = viewResult.Model.Should().BeOfType<ErrorViewModel>().Subject;
        model.Message.Should().Be("Invalid staff ID.");

        _apiClientMock.Verify(api => api.DeleteAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeletePost_ShouldReturnErrorView_WhenApiDeleteFails()
    {
        // Arrange
        int staffId = 10;
        const string expectedViewName = "Error";
        const string errorMessage = "Delete failed";

        var deleteModel = new StaffManagementDeleteViewModel
        {
            BasicInfo = new StaffBasicViewModel
            {
                Id = staffId,
                EmailAddress = "someone@example.com"
            }
        };

        _apiClientMock
            .Setup(api => api.DeleteAsync($"staff/{staffId}"))
            .ReturnsAsync(Result<bool>.Failure(errorMessage));

        // Act
        IActionResult result = await _sut.Delete(deleteModel);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        var model = viewResult.Model.Should().BeOfType<ErrorViewModel>().Subject;
        model.Message.Should().Be(errorMessage);

        _userServiceMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Never);
    }
}
