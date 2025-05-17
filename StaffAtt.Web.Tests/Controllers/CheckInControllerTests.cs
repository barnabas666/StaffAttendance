using AutoMapper;
using FluentAssertions;
using Moq;
using StaffAtt.Web.Controllers;
using StaffAtt.Web.Helpers;
using StaffAttLibrary.Data;
using StaffAtt.Web.Models;
using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StaffAtt.Web.Tests.Controllers;
public class CheckInControllerTests
{
    private readonly CheckInController _sut;
    private readonly Mock<IStaffService> _staffServiceMock = new();
    private readonly Mock<ICheckInService> _checkInServiceMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IStaffSelectListService> _staffSelectListServiceMock = new();
    public CheckInControllerTests()
    {
        _sut = new CheckInController(
            _staffServiceMock.Object,
            _checkInServiceMock.Object,
            _userContextMock.Object,
            _mapperMock.Object,
            _staffSelectListServiceMock.Object
        );
    }

    [Fact]
    public async Task List_ShouldReturnListViewWithCheckInDisplayAdminViewModel()
    {
        // Arrange
        string expectedViewName = "List";
        CheckInDisplayAdminViewModel checkInDisplayAdminViewModel = new CheckInDisplayAdminViewModel();
        List<CheckInFullModel> checkIns = new List<CheckInFullModel>();
        _checkInServiceMock.Setup(m => m.GetAllCheckInsByDateAsync(checkInDisplayAdminViewModel.StartDate,
                                                                   checkInDisplayAdminViewModel.EndDate))
            .ReturnsAsync(checkIns);
        _mapperMock.Setup(m => m.Map<List<CheckInFullViewModel>>(checkIns))
            .Returns(checkInDisplayAdminViewModel.CheckIns);
        List<StaffBasicModel> basicStaff = new List<StaffBasicModel>();
        _staffServiceMock.Setup(m => m.GetAllBasicStaffAsync())
            .ReturnsAsync(basicStaff);
        _mapperMock.Setup(m => m.Map<List<StaffBasicViewModel>>(basicStaff))
            .Returns(checkInDisplayAdminViewModel.StaffList);
        // Act
        IActionResult result = await _sut.List();
        // Assert
        ViewResult viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        CheckInDisplayAdminViewModel model = viewResult.Model.Should().BeOfType<CheckInDisplayAdminViewModel>().Subject;
    }

    [Fact]
    public async Task ListPost_ShouldReturnListViewWithCheckInDisplayAdminViewModel()
    {
        // Arrange
        string expectedViewName = "List";
        CheckInDisplayAdminViewModel checkInDisplayAdminViewModel = new CheckInDisplayAdminViewModel();
        checkInDisplayAdminViewModel.SelectedStaffId = "0"; // All Staff selected
        List<CheckInFullModel> checkIns = new List<CheckInFullModel>();
        _checkInServiceMock.Setup(m => m.GetAllCheckInsByDateAsync(checkInDisplayAdminViewModel.StartDate,
                                                                   checkInDisplayAdminViewModel.EndDate))
            .ReturnsAsync(checkIns);
        _mapperMock.Setup(m => m.Map<List<CheckInFullViewModel>>(checkIns))
            .Returns(checkInDisplayAdminViewModel.CheckIns);
        List<StaffBasicModel> basicStaff = new List<StaffBasicModel>();
        _staffServiceMock.Setup(m => m.GetAllBasicStaffAsync())
            .ReturnsAsync(basicStaff);
        _mapperMock.Setup(m => m.Map<List<StaffBasicViewModel>>(basicStaff))
            .Returns(checkInDisplayAdminViewModel.StaffList);
        // Act
        IActionResult result = await _sut.List(checkInDisplayAdminViewModel);
        // Assert
        ViewResult viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        CheckInDisplayAdminViewModel model = viewResult.Model.Should().BeOfType<CheckInDisplayAdminViewModel>().Subject;
    }

    [Fact]
    public async Task ListPost_ShouldCallGetAllCheckInsByDateAsyncMethod_WhenAllStaffIsSelected()
    {
        // Arrange
        string expectedViewName = "List";
        CheckInDisplayAdminViewModel checkInDisplayAdminViewModel = new CheckInDisplayAdminViewModel();
        checkInDisplayAdminViewModel.SelectedStaffId = "0"; // All Staff selected
        List<CheckInFullModel> checkIns = new List<CheckInFullModel>();
        _checkInServiceMock.Setup(m => m.GetAllCheckInsByDateAsync(checkInDisplayAdminViewModel.StartDate,
                                                                   checkInDisplayAdminViewModel.EndDate))
            .ReturnsAsync(checkIns);
        _mapperMock.Setup(m => m.Map<List<CheckInFullViewModel>>(checkIns))
            .Returns(checkInDisplayAdminViewModel.CheckIns);
        List<StaffBasicModel> basicStaff = new List<StaffBasicModel>();
        _staffServiceMock.Setup(m => m.GetAllBasicStaffAsync())
            .ReturnsAsync(basicStaff);
        _mapperMock.Setup(m => m.Map<List<StaffBasicViewModel>>(basicStaff))
            .Returns(checkInDisplayAdminViewModel.StaffList);
        // Act
        IActionResult result = await _sut.List(checkInDisplayAdminViewModel);
        // Assert
        _checkInServiceMock.Verify(m => m.GetAllCheckInsByDateAsync(checkInDisplayAdminViewModel.StartDate,
                                                                   checkInDisplayAdminViewModel.EndDate), Times.Once);
    }

    [Fact]
    public async Task ListPost_ShouldCallGetCheckInsByDateAndIdAsyncMethod_WhenSingleStaffIsSelected()
    {
        // Arrange
        string expectedViewName = "List";
        CheckInDisplayAdminViewModel checkInDisplayAdminViewModel = new CheckInDisplayAdminViewModel();
        checkInDisplayAdminViewModel.SelectedStaffId = "1"; // Single Staff selected
        List<CheckInFullModel> checkIns = new List<CheckInFullModel>();
        _checkInServiceMock.Setup(m => m.GetCheckInsByDateAndIdAsync(Convert.ToInt32(checkInDisplayAdminViewModel.SelectedStaffId),
                                                                     checkInDisplayAdminViewModel.StartDate,
                                                                     checkInDisplayAdminViewModel.EndDate))
            .ReturnsAsync(checkIns);
        _mapperMock.Setup(m => m.Map<List<CheckInFullViewModel>>(checkIns))
            .Returns(checkInDisplayAdminViewModel.CheckIns);
        List<StaffBasicModel> basicStaff = new List<StaffBasicModel>();
        _staffServiceMock.Setup(m => m.GetAllBasicStaffAsync())
            .ReturnsAsync(basicStaff);
        _mapperMock.Setup(m => m.Map<List<StaffBasicViewModel>>(basicStaff))
            .Returns(checkInDisplayAdminViewModel.StaffList);
        // Act
        IActionResult result = await _sut.List(checkInDisplayAdminViewModel);
        // Assert
        _checkInServiceMock.Verify(m => m.GetCheckInsByDateAndIdAsync(Convert.ToInt32(checkInDisplayAdminViewModel.SelectedStaffId),
                                                                      checkInDisplayAdminViewModel.StartDate,
                                                                      checkInDisplayAdminViewModel.EndDate), Times.Once);
    }

    [Fact]
    public async Task Display_ShouldReturnDisplayViewWithCheckInDisplayStaffViewModel()
    {
        // Arrange
        string expectedViewName = "Display";
        string userEmail = "john.doe@johndoe.com";
        _userContextMock.Setup(x => x.GetUserEmail()).Returns(userEmail);
        CheckInDisplayStaffViewModel checkInDisplayStaffViewModel = new CheckInDisplayStaffViewModel();
        List<CheckInFullModel> checkIns = new List<CheckInFullModel>();
        _checkInServiceMock.Setup(m => m.GetCheckInsByDateAndEmailAsync(userEmail,
                                                                      checkInDisplayStaffViewModel.StartDate,
                                                                      checkInDisplayStaffViewModel.EndDate))
            .ReturnsAsync(checkIns);
        _mapperMock.Setup(m => m.Map<List<CheckInFullViewModel>>(checkIns))
            .Returns(checkInDisplayStaffViewModel.CheckIns);
        // Act
        IActionResult result = await _sut.Display();
        // Assert
        ViewResult viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        CheckInDisplayStaffViewModel model = viewResult.Model.Should().BeOfType<CheckInDisplayStaffViewModel>().Subject;
    }

    [Fact]
    public async Task DisplayPost_ShouldReturnDisplayViewWithCheckInDisplayStaffViewModel()
    {
        // Arrange
        string expectedViewName = "Display";
        string userEmail = "john.doe@johndoe.com";
        _userContextMock.Setup(x => x.GetUserEmail()).Returns(userEmail);
        CheckInDisplayStaffViewModel checkInDisplayStaffViewModel = new CheckInDisplayStaffViewModel();
        List<CheckInFullModel> checkIns = new List<CheckInFullModel>();
        _checkInServiceMock.Setup(m => m.GetCheckInsByDateAndEmailAsync(userEmail,
                                                                      checkInDisplayStaffViewModel.StartDate,
                                                                      checkInDisplayStaffViewModel.EndDate))
            .ReturnsAsync(checkIns);
        _mapperMock.Setup(m => m.Map<List<CheckInFullViewModel>>(checkIns))
            .Returns(checkInDisplayStaffViewModel.CheckIns);
        // Act
        IActionResult result = await _sut.Display(checkInDisplayStaffViewModel);
        // Assert
        ViewResult viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        CheckInDisplayStaffViewModel model = viewResult.Model.Should().BeOfType<CheckInDisplayStaffViewModel>().Subject;
    }
}
