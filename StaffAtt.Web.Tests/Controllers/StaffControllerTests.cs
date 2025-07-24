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
using StaffAttLibrary.Models;

namespace StaffAtt.Web.Tests.Controllers;
public class StaffControllerTests
{
    private readonly StaffController _sut;
    private readonly Mock<IStaffService> _staffServiceMock = new();
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IPhoneNumberParser> _phoneNumberParserMock = new();
    private readonly Mock<IDepartmentSelectListService> _departmentSelectListServiceMock = new();

    public StaffControllerTests()
    {
        _sut = new StaffController(
            _staffServiceMock.Object,
            _userServiceMock.Object,
            _userContextMock.Object,
            _mapperMock.Object,
            _phoneNumberParserMock.Object,
            _departmentSelectListServiceMock.Object
        );
    }

    [Fact]
    public async Task Create_ShouldReturnCreateViewWithStaffCreateViewModel()
    {
        // Arrange
        string expectedViewName = "Create";
        List<DepartmentModel> departments = new List<DepartmentModel>
        {
            new DepartmentModel { Id = 1, Title = "IT", Description = "IT department." },
            new DepartmentModel { Id = 2, Title = "HR", Description = "Human resources department." }
        };
        SelectList selectListItems = new SelectList(departments, nameof(DepartmentModel.Id), nameof(DepartmentModel.Title));
        _departmentSelectListServiceMock.Setup(x => x.GetDepartmentSelectListAsync(It.IsAny<string>()))
            .ReturnsAsync(selectListItems);
        // Act
        IActionResult result = await _sut.Create();
        // Assert
        ViewResult viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        StaffCreateViewModel model = viewResult.Model.Should().BeAssignableTo<StaffCreateViewModel>().Subject;
        model.DepartmentItems.Should().BeEquivalentTo(selectListItems);
    }

    [Fact]
    public async Task CreatePost_ShouldRedirectToDetailsActionWithoutRouteValues_WhenUserHasNoAccountYet()
    {
        // Arrange
        string expectedActionName = "Details";
        StaffCreateViewModel staffCreateModel = new StaffCreateViewModel
        {
            FirstName = "John",
            LastName = "Doe",
            PIN = 1234,
            DepartmentId = "1",
            Address = new AddressViewModel(),
            PhoneNumbersText = "111222333,444555666",
        };
        string userEmail = "john.doe@johndoe.com";
        _userContextMock.Setup(x => x.GetUserEmail()).Returns(userEmail);
        _staffServiceMock.Setup(x => x.CheckStaffByEmailAsync(userEmail)).ReturnsAsync(false);
        _phoneNumberParserMock.Setup(x => x.ParseStringToPhoneNumbers(It.IsAny<string>()))
            .Returns(It.IsAny<List<PhoneNumberModel>>);
        _mapperMock.Setup(AddressViewModel => AddressViewModel.Map<AddressModel>(It.IsAny<AddressViewModel>()))
            .Returns(It.IsAny<AddressModel>());
        _staffServiceMock.Setup(x => x.CreateStaffAsync(Convert.ToInt32(staffCreateModel.DepartmentId),
                                                        It.IsAny<AddressModel>(),
                                                        staffCreateModel.PIN.ToString(),
                                                        staffCreateModel.FirstName,
                                                        staffCreateModel.LastName,
                                                        userEmail,
                                                        It.IsAny<List<PhoneNumberModel>>()))
                                                        .Returns(Task.CompletedTask);
        IdentityUser identityUser = new IdentityUser { Email = userEmail };
        _userServiceMock.Setup(x => x.FindByEmailAsync(userEmail)).ReturnsAsync(identityUser);
        _userServiceMock.Setup(x => x.AddToRoleAsync(identityUser, "Member")).ReturnsAsync(IdentityResult.Success);
        _userServiceMock.Setup(x => x.SignInAsync(identityUser, false)).Returns(Task.CompletedTask);
        // Act
        IActionResult result = await _sut.Create(staffCreateModel);
        // Assert
        RedirectToActionResult redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectToActionResult.ActionName.Should().Be(expectedActionName);
        redirectToActionResult.RouteValues.Should().BeNull();
    }

    [Fact]
    public async Task CreatePost_ShouldRedirectToDetailsActionWithRouteValues_WhenUserHasAlreadyAccount()
    {
        // Arrange
        string expectedActionName = "Details";
        string expectedRouteName = "message";
        string expectedMessage = "You have already created account!";
        StaffCreateViewModel staffCreateModel = new StaffCreateViewModel();
        string userEmail = "john.doe@johndoe.com";
        _userContextMock.Setup(x => x.GetUserEmail()).Returns(userEmail);
        _staffServiceMock.Setup(x => x.CheckStaffByEmailAsync(userEmail)).ReturnsAsync(true);
        // Act
        IActionResult result = await _sut.Create(staffCreateModel);
        // Assert
        RedirectToActionResult redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectToActionResult.ActionName.Should().Be(expectedActionName);
        redirectToActionResult.RouteValues[expectedRouteName].Should().Be(expectedMessage);
    }

    [Fact]
    public async Task Details_ShouldReturnDetailsViewWithStaffDetailsViewModel_WhenUserHasAlreadyAccount()
    {
        // Arrange
        string expectedViewName = "Details";
        string expectedMessage = "expected message";
        string userEmail = "john.doe@johndoe.com";
        _userContextMock.Setup(x => x.GetUserEmail()).Returns(userEmail);
        _staffServiceMock.Setup(x => x.CheckStaffByEmailAsync(userEmail)).ReturnsAsync(true);
        _staffServiceMock.Setup(x => x.GetStaffByEmailAsync(userEmail)).ReturnsAsync(It.IsAny<StaffFullModel>());
        StaffDetailsViewModel expectedDetailsModel = new StaffDetailsViewModel();
        _mapperMock.Setup(x => x.Map<StaffDetailsViewModel>(It.IsAny<StaffFullModel>()))
            .Returns(expectedDetailsModel);
        expectedDetailsModel.Message = expectedMessage;
        // Act
        IActionResult result = await _sut.Details(expectedMessage);
        // Assert
        ViewResult viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        StaffDetailsViewModel model = viewResult.Model.Should().BeAssignableTo<StaffDetailsViewModel>().Subject;
        model.Message.Should().Be(expectedMessage);
    }

    [Fact]
    public async Task Details_ShouldRedirectToCreateAction_WhenUserHasNoAccountYet()
    {
        // Arrange
        string expectedActionName = "Create";
        string userEmail = "john.doe@johndoe.com";
        _userContextMock.Setup(x => x.GetUserEmail()).Returns(userEmail);
        _staffServiceMock.Setup(x => x.CheckStaffByEmailAsync(userEmail)).ReturnsAsync(false);
        // Act
        IActionResult result = await _sut.Details(It.IsAny<string>());
        // Assert
        RedirectToActionResult redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectToActionResult.ActionName.Should().Be(expectedActionName);
    }

    [Fact]
    public async Task Update_ShouldReturnUpdateViewWithStaffUpdateViewModel()
    {
        // Arrange
        string expectedViewName = "Update";
        string userEmail = "john.doe@johndoe.com";
        StaffFullModel fullModel = new StaffFullModel();
        StaffUpdateViewModel expectedUpdateModel = new StaffUpdateViewModel();
        _userContextMock.Setup(x => x.GetUserEmail()).Returns(userEmail);
        _staffServiceMock.Setup(x => x.GetStaffByEmailAsync(userEmail)).ReturnsAsync(fullModel);
        _mapperMock.Setup(x => x.Map<StaffUpdateViewModel>(fullModel))
            .Returns(expectedUpdateModel);
        _phoneNumberParserMock.Setup(x => x.ParsePhoneNumbersToString(fullModel.PhoneNumbers))
            .Returns(expectedUpdateModel.PhoneNumbersText);
        // Act
        IActionResult result = await _sut.Update();
        // Assert
        ViewResult viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        StaffUpdateViewModel model = viewResult.Model.Should().BeAssignableTo<StaffUpdateViewModel>().Subject;
    }

    [Fact]
    public async Task UpdatePost_ShouldRedirectToDetailsAction()
    {
        // Arrange
        string expectedActionName = "Details";
        string userEmail = "john.doe@johndoe.com";
        _userContextMock.Setup(x => x.GetUserEmail()).Returns(userEmail);
        StaffUpdateViewModel staffUpdateModel = new StaffUpdateViewModel
        {
            FirstName = "John",
            LastName = "Doe",
            PIN = 1234,
            Address = new AddressViewModel(),
            EmailAddress = userEmail,
            PhoneNumbersText = "111222333,444555666",
        };
        List<PhoneNumberModel> phoneNumbers = new List<PhoneNumberModel>();
        _phoneNumberParserMock.Setup(x => x.ParseStringToPhoneNumbers(staffUpdateModel.PhoneNumbersText))
            .Returns(phoneNumbers);
        AddressModel address = new AddressModel();
        _mapperMock.Setup(x => x.Map<AddressModel>(staffUpdateModel.Address))
            .Returns(address);
        _staffServiceMock.Setup(x => x.UpdateStaffAsync(address,
                                                        staffUpdateModel.PIN.ToString(),
                                                        staffUpdateModel.FirstName,
                                                        staffUpdateModel.LastName,
                                                        userEmail,
                                                        phoneNumbers))
            .Returns(Task.CompletedTask);
        // Act
        IActionResult result = await _sut.Update(staffUpdateModel);
        // Assert
        RedirectToActionResult redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectToActionResult.ActionName.Should().Be(expectedActionName);
    }
}
