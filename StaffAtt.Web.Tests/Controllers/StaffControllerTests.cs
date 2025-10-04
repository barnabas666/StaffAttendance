using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using StaffAtt.Web.Controllers;
using StaffAtt.Web.Helpers;
using StaffAtt.Web.Models;
using StaffAttLibrary.Models;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Tests.Controllers;
public class StaffControllerTests
{
    private readonly StaffController _sut;
    private readonly Mock<IApiClient> _apiClientMock = new();
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IPhoneNumberDtoParser> _phoneNumberParserMock = new();
    private readonly Mock<IDepartmentSelectListService> _departmentSelectListServiceMock = new();

    public StaffControllerTests()
    {
        _sut = new StaffController(
            _apiClientMock.Object,
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
        const string expectedViewName = "Create";

        var departments = new List<DepartmentModel>
            {
                new DepartmentModel { Id = 1, Title = "IT" },
                new DepartmentModel { Id = 2, Title = "HR" }
            };

        var selectListItems = new SelectList(departments, nameof(DepartmentModel.Id), nameof(DepartmentModel.Title));

        _departmentSelectListServiceMock
            .Setup(x => x.GetDepartmentSelectListAsync(It.IsAny<string>()))
            .ReturnsAsync(selectListItems);

        // Act
        var result = await _sut.Create();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);

        var model = viewResult.Model.Should().BeAssignableTo<StaffCreateViewModel>().Subject;
        model.DepartmentItems.Should().BeEquivalentTo(selectListItems);
    }

    [Fact]
    public async Task CreatePost_ShouldRedirectToDetailsActionWithoutRouteValues_WhenUserHasNoAccountYet()
    {
        // Arrange
        const string expectedActionName = "Details";
        var staffCreateModel = new StaffCreateViewModel
        {
            FirstName = "John",
            LastName = "Doe",
            PIN = 1234,
            DepartmentId = "1",
            Address = new AddressViewModel(),
            PhoneNumbersText = "111222333,444555666",
        };

        var userEmail = "john.doe@johndoe.com";

        _userContextMock.Setup(x => x.GetUserEmail()).Returns(userEmail);

        // API call: user does NOT have an account
        _apiClientMock
            .Setup(x => x.GetAsync<bool>($"staff/check-email?emailAddress={Uri.EscapeDataString(userEmail)}"))
            .ReturnsAsync(Result<bool>.Success(false));

        // API call: create staff
        _apiClientMock
            .Setup(x => x.PostAsync<CreateStaffRequest>("staff", It.IsAny<CreateStaffRequest>()))
            .ReturnsAsync(Result<CreateStaffRequest>.Success(null));

        // Mapping and parsing
        _phoneNumberParserMock
            .Setup(x => x.ParseStringToPhoneNumbers(It.IsAny<string>()))
            .Returns(new List<PhoneNumberDto>());

        _mapperMock
            .Setup(x => x.Map<AddressDto>(It.IsAny<AddressViewModel>()))
            .Returns(new AddressDto());

        // Identity setup
        var identityUser = new IdentityUser { Email = userEmail };
        _userServiceMock.Setup(x => x.FindByEmailAsync(userEmail)).ReturnsAsync(identityUser);
        _userServiceMock.Setup(x => x.AddToRoleAsync(identityUser, "Member")).ReturnsAsync(IdentityResult.Success);
        _userServiceMock.Setup(x => x.SignInAsync(identityUser, false)).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Create(staffCreateModel);

        // Assert
        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be(expectedActionName);
        redirect.RouteValues.Should().BeNull();
    }

    [Fact]
    public async Task CreatePost_ShouldRedirectToDetailsActionWithRouteValues_WhenUserHasAlreadyAccount()
    {
        // Arrange
        const string expectedActionName = "Details";
        const string expectedRouteName = "message";
        const string expectedMessage = "You have already created account!";

        var staffCreateModel = new StaffCreateViewModel();
        var userEmail = "john.doe@johndoe.com";

        _userContextMock.Setup(x => x.GetUserEmail()).Returns(userEmail);

        // API call: user ALREADY has an account
        _apiClientMock
            .Setup(x => x.GetAsync<bool>($"staff/check-email?emailAddress={Uri.EscapeDataString(userEmail)}"))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _sut.Create(staffCreateModel);

        // Assert
        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be(expectedActionName);
        redirect.RouteValues[expectedRouteName].Should().Be(expectedMessage);
    }

    [Fact]
    public async Task Details_ShouldReturnDetailsViewWithStaffDetailsViewModel_WhenUserHasAlreadyAccount()
    {
        // Arrange
        const string expectedViewName = "Details";
        const string expectedMessage = "expected message";
        var userEmail = "john.doe@johndoe.com";

        _userContextMock.Setup(x => x.GetUserEmail()).Returns(userEmail);

        // API mock: user exists
        _apiClientMock
            .Setup(x => x.GetAsync<bool>($"staff/check-email?emailAddress={Uri.EscapeDataString(userEmail)}"))
            .ReturnsAsync(Result<bool>.Success(true));

        // API mock: get staff details
        var staffFullDto = new StaffFullDto
        {
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = userEmail
        };

        _apiClientMock
            .Setup(x => x.GetAsync<StaffFullDto>($"staff/email?emailAddress={Uri.EscapeDataString(userEmail)}"))
            .ReturnsAsync(Result<StaffFullDto>.Success(staffFullDto));

        // Mapping to ViewModel
        var expectedDetailsModel = new StaffDetailsViewModel();
        _mapperMock
            .Setup(x => x.Map<StaffDetailsViewModel>(staffFullDto))
            .Returns(expectedDetailsModel);

        // Act
        var result = await _sut.Details(expectedMessage);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);

        var model = viewResult.Model.Should().BeAssignableTo<StaffDetailsViewModel>().Subject;
        model.Should().BeEquivalentTo(expectedDetailsModel);
        model.Message.Should().Be(expectedMessage);
    }

    [Fact]
    public async Task Details_ShouldRedirectToCreateAction_WhenUserHasNoAccountYet()
    {
        // Arrange
        const string expectedActionName = "Create";
        var userEmail = "john.doe@johndoe.com";

        _userContextMock.Setup(x => x.GetUserEmail()).Returns(userEmail);

        // API mock: user does NOT exist
        _apiClientMock
            .Setup(x => x.GetAsync<bool>($"staff/check-email?emailAddress={Uri.EscapeDataString(userEmail)}"))
            .ReturnsAsync(Result<bool>.Success(false));

        // Act
        var result = await _sut.Details(It.IsAny<string>());

        // Assert
        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be(expectedActionName);
    }

    [Fact]
    public async Task Update_ShouldReturnUpdateViewWithStaffUpdateViewModel()
    {
        // Arrange
        const string expectedViewName = "Update";
        var userEmail = "john.doe@johndoe.com";

        _userContextMock.Setup(x => x.GetUserEmail()).Returns(userEmail);

        var staffDto = new StaffFullDto
        {
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = userEmail,
            PhoneNumbers = new List<PhoneNumberDto>
                {
                    new PhoneNumberDto { Id = 1, PhoneNumber = "111222333" }
                }
        };

        // API mock: get staff data
        _apiClientMock
            .Setup(x => x.GetAsync<StaffFullDto>($"staff/email?emailAddress={Uri.EscapeDataString(userEmail)}"))
            .ReturnsAsync(Result<StaffFullDto>.Success(staffDto));

        // Mapping
        var expectedUpdateModel = new StaffUpdateViewModel();
        _mapperMock.Setup(x => x.Map<StaffUpdateViewModel>(staffDto))
            .Returns(expectedUpdateModel);

        _phoneNumberParserMock
            .Setup(x => x.ParsePhoneNumbersToString(staffDto.PhoneNumbers))
            .Returns("111222333");

        // Act
        var result = await _sut.Update();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        viewResult.Model.Should().BeAssignableTo<StaffUpdateViewModel>();
    }

    [Fact]
    public async Task UpdatePost_ShouldRedirectToDetailsAction()
    {
        // Arrange
        const string expectedActionName = "Details";
        var userEmail = "john.doe@johndoe.com";

        _userContextMock.Setup(x => x.GetUserEmail()).Returns(userEmail);

        var updateModel = new StaffUpdateViewModel
        {
            FirstName = "John",
            LastName = "Doe",
            PIN = 1234,
            Address = new AddressViewModel(),
            EmailAddress = userEmail,
            PhoneNumbersText = "111222333,444555666"
        };

        // Parse and map
        _phoneNumberParserMock
            .Setup(x => x.ParseStringToPhoneNumbers(updateModel.PhoneNumbersText))
            .Returns(new List<PhoneNumberDto>());

        _mapperMock
            .Setup(x => x.Map<AddressDto>(updateModel.Address))
            .Returns(new AddressDto());

        // API call to update staff
        _apiClientMock
            .Setup(x => x.PutAsync<UpdateStaffRequest>("staff", It.IsAny<UpdateStaffRequest>()))
            .ReturnsAsync(Result<UpdateStaffRequest>.Success(null));

        // Act
        var result = await _sut.Update(updateModel);

        // Assert
        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be(expectedActionName);
    }
}
