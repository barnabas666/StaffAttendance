using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using StaffAtt.Web.Controllers;
using StaffAtt.Web.Helpers;
using StaffAtt.Web.Models;
using StaffAttLibrary.Data;
using StaffAttLibrary.Enums;
using StaffAttLibrary.Models;

namespace StaffAtt.Web.Tests.Controllers;
public class StaffManagementControllerTests
{
    private readonly StaffManagementController _sut;
    private readonly Mock<IStaffService> _staffServiceMock = new();
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IDepartmentSelectListService> _departmentSelectListServiceMock = new();

    public StaffManagementControllerTests()
    {
        _sut = new StaffManagementController(
            _staffServiceMock.Object,
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
        StaffManagementListViewModel staffManagementListViewModel = new StaffManagementListViewModel();
        List<StaffBasicModel> staffBasicModels = new List<StaffBasicModel>();
        _staffServiceMock.Setup(m => m.GetAllBasicStaffAsync()).
            ReturnsAsync(staffBasicModels);
        _mapperMock.Setup(m => m.Map<List<StaffBasicViewModel>>(staffBasicModels))
            .Returns(staffManagementListViewModel.BasicInfos);
        List<DepartmentModel> departments = new List<DepartmentModel>
        {
            new DepartmentModel { Id = 1, Title = "IT", Description = "IT department." },
            new DepartmentModel { Id = 2, Title = "HR", Description = "Human resources department." }
        };
        SelectList selectListItems = new SelectList(departments, nameof(DepartmentModel.Id), nameof(DepartmentModel.Title));
        _departmentSelectListServiceMock.Setup(m => m.GetDepartmentSelectListAsync("All"))
            .ReturnsAsync(selectListItems);
        // Act
        IActionResult result = await _sut.List();
        // Assert
        ViewResult viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        StaffManagementListViewModel model = viewResult.Model.Should().BeOfType<StaffManagementListViewModel>().Subject;
        model.DepartmentItems.Should().BeEquivalentTo(selectListItems);
    }

    [Fact]
    public async Task ListPost_ShouldReturnListViewWithStaffManagementListViewModel()
    {
        // Arrange
        string expectedViewName = "List";
        StaffManagementListViewModel staffModel = new StaffManagementListViewModel
        {
            DepartmentId = "1",
            ApprovedRadio = ApprovedType.Approved
        };
        List<StaffBasicModel> staffBasicModels = new List<StaffBasicModel>();
        _staffServiceMock.Setup(m => m.GetAllBasicStaffFilteredAsync(Convert.ToInt32(staffModel.DepartmentId),
            staffModel.ApprovedRadio)).ReturnsAsync(staffBasicModels);
        _mapperMock.Setup(m => m.Map<List<StaffBasicViewModel>>(staffBasicModels))
            .Returns(staffModel.BasicInfos);
        List<DepartmentModel> departments = new List<DepartmentModel>
        {
            new DepartmentModel { Id = 1, Title = "IT", Description = "IT department." },
            new DepartmentModel { Id = 2, Title = "HR", Description = "Human resources department." }
        };
        SelectList selectListItems = new SelectList(departments, nameof(DepartmentModel.Id), nameof(DepartmentModel.Title));
        _departmentSelectListServiceMock.Setup(m => m.GetDepartmentSelectListAsync("All"))
            .ReturnsAsync(selectListItems);
        // Act
        IActionResult result = await _sut.List(staffModel);
        // Assert
        ViewResult viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        StaffManagementListViewModel model = viewResult.Model.Should().BeOfType<StaffManagementListViewModel>().Subject;
        model.DepartmentItems.Should().BeEquivalentTo(selectListItems);
    }

    [Fact]
    public async Task Details_ShouldReturnDetailsViewWithStaffDetailsViewModel()
    {
        // Arrange
        int staffId = 1;
        string expectedViewName = "Details";
        StaffFullModel staffFullModel = new StaffFullModel { Id = staffId };
        _staffServiceMock.Setup(m => m.GetStaffByIdAsync(staffId)).ReturnsAsync(staffFullModel);
        StaffDetailsViewModel detailsModel = new StaffDetailsViewModel
        {
            BasicInfo = new StaffBasicViewModel { Id = staffId }
        };
        _mapperMock.Setup(m => m.Map<StaffDetailsViewModel>(staffFullModel))
            .Returns(detailsModel);
        // Act
        IActionResult result = await _sut.Details(staffId);
        // Assert
        ViewResult viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        StaffDetailsViewModel model = viewResult.Model.Should().BeOfType<StaffDetailsViewModel>().Subject;
    }

    [Fact]
    public async Task Update_ShouldReturnUpdateViewWithStaffManagementUpdateViewModel()
    {
        // Arrange
        int staffId = 1;
        string expectedViewName = "Update";
        StaffBasicModel staffBasicModel = new StaffBasicModel { Id = staffId };
        _staffServiceMock.Setup(m => m.GetBasicStaffByIdAsync(staffId)).ReturnsAsync(staffBasicModel);
        StaffManagementUpdateViewModel updateModel = new StaffManagementUpdateViewModel
        {
            BasicInfo = new StaffBasicViewModel { Id = staffId }
        };
        _mapperMock.Setup(m => m.Map<StaffManagementUpdateViewModel>(staffBasicModel))
            .Returns(updateModel);
        // Act
        IActionResult result = await _sut.Update(staffId);
        // Assert
        ViewResult viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        StaffManagementUpdateViewModel model = viewResult.Model.Should().BeOfType<StaffManagementUpdateViewModel>().Subject;
    }

    [Fact]
    public async Task UpdatePost_ShouldRedirectToListAction()
    {
        // Arrange
        int staffId = 1;
        string expectedActionName = "List";
        StaffManagementUpdateViewModel updateModel = new StaffManagementUpdateViewModel
        {
            BasicInfo = new StaffBasicViewModel
            {
                Id = staffId,
                DepartmentId = 1,
                IsApproved = true
            }
        };
        // Act
        IActionResult result = await _sut.Update(updateModel);
        // Assert
        RedirectToActionResult redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be(expectedActionName);
    }

    [Fact]
    public async Task Delete_ShouldReturnDeleteViewWithStaffManagementDeleteViewModel()
    {
        // Arrange
        int staffId = 1;
        string expectedViewName = "Delete";
        StaffBasicModel staffBasicModel = new StaffBasicModel { Id = staffId };
        _staffServiceMock.Setup(m => m.GetBasicStaffByIdAsync(staffId)).ReturnsAsync(staffBasicModel);
        StaffManagementDeleteViewModel deleteModel = new StaffManagementDeleteViewModel
        {
            BasicInfo = new StaffBasicViewModel { Id = staffId }
        };
        _mapperMock.Setup(m => m.Map<StaffManagementDeleteViewModel>(staffBasicModel))
            .Returns(deleteModel);
        // Act
        IActionResult result = await _sut.Delete(staffId);
        // Assert
        ViewResult viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        StaffManagementDeleteViewModel model = viewResult.Model.Should().BeOfType<StaffManagementDeleteViewModel>().Subject;
    }

    [Fact]
    public async Task DeletePost_ShoudRedirectToListAction()
    {
        // Arrange
        int staffId = 1;
        string userEmail = "john.doe@johndoe.com";
        StaffBasicModel staffBasicModel = new StaffBasicModel { Id = staffId };
        IdentityUser identityUser = new IdentityUser { Email = userEmail };
        string expectedActionName = "List";
        _staffServiceMock.Setup(m => m.GetStaffEmailByIdAsync(staffId)).ReturnsAsync(userEmail);
        _staffServiceMock.Setup(m => m.DeleteStaffAsync(staffId)).Returns(Task.CompletedTask);
        _userServiceMock.Setup(x => x.FindByEmailAsync(userEmail)).ReturnsAsync(identityUser);
        _userServiceMock.Setup(x => x.DeleteIdentityUserAsync(identityUser)).Returns(Task.CompletedTask);
        // Act
        IActionResult result = await _sut.Delete(staffBasicModel);
        // Assert
        RedirectToActionResult redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be(expectedActionName);
    }
}
