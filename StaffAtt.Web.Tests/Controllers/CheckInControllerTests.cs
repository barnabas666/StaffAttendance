using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using StaffAtt.Web.Controllers;
using StaffAtt.Web.Helpers;
using StaffAtt.Web.Models;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Tests.Controllers;
public class CheckInControllerTests
{
    private readonly CheckInController _sut;
    private readonly Mock<IApiClient> _apiClientMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IStaffSelectListService> _staffSelectListServiceMock = new();

    public CheckInControllerTests()
    {
        _sut = new CheckInController(
            _apiClientMock.Object,
            _userContextMock.Object,
            _mapperMock.Object,
            _staffSelectListServiceMock.Object
        );
    }

    [Fact]
    public async Task List_ShouldReturnListViewWithCheckInDisplayAdminViewModel()
    {
        // Arrange
        const string expectedViewName = "List";

        // Prepare DTOs returned by API
        var checkInDtos = new List<CheckInFullDto>
        {
            new CheckInFullDto { Id = 1, FirstName = "John", LastName = "Doe", EmailAddress = "john@example.com", Title = "IT", StaffId = 1, CheckInDate = DateTime.Today }
        };

        var staffDtos = new List<StaffBasicDto>
        {
            new StaffBasicDto { Id = 1, FirstName = "John", LastName = "Doe", EmailAddress = "john@example.com", Title = "IT", DepartmentId = 1 }
        };

        // Mock API call for checkins: match any "checkin/all?startDate=" prefix (avoid strict date matching)
        _apiClientMock
            .Setup(api => api.GetAsync<List<CheckInFullDto>>(It.Is<string>(s => s.StartsWith("checkin/all?startDate="))))
            .ReturnsAsync(Result<List<CheckInFullDto>>.Success(checkInDtos));

        // Mock API call for staff list
        _apiClientMock
            .Setup(api => api.GetAsync<List<StaffBasicDto>>("staff/basic"))
            .ReturnsAsync(Result<List<StaffBasicDto>>.Success(staffDtos));

        // Map DTOs to view models (controller will call these)
        var mappedCheckInViewModels = new List<CheckInFullViewModel>
        {
            new CheckInFullViewModel { Id = 1, FirstName = "John", LastName = "Doe", EmailAddress = "john@example.com", Title = "IT" }
        };
        _mapperMock
            .Setup(m => m.Map<List<CheckInFullViewModel>>(checkInDtos))
            .Returns(mappedCheckInViewModels);

        var mappedStaffViewModels = new List<StaffBasicViewModel>
        {
            new StaffBasicViewModel { Id = 1, FirstName = "John", LastName = "Doe", EmailAddress = "john@example.com", Title = "IT", DepartmentId = 1 }
        };
        _mapperMock
            .Setup(m => m.Map<List<StaffBasicViewModel>>(staffDtos))
            .Returns(mappedStaffViewModels);

        // Staff dropdown service (controller calls this — return any SelectList)
        _staffSelectListServiceMock
            .Setup(s => s.GetStaffSelectListAsync(It.IsAny<CheckInDisplayAdminViewModel>(), "All Staff"))
            .ReturnsAsync(new SelectList(new List<StaffBasicViewModel>(), "Id", "FullName"));

        // Act
        IActionResult result = await _sut.List();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);

        var model = viewResult.Model.Should().BeOfType<CheckInDisplayAdminViewModel>().Subject;
        model.CheckIns.Should().BeEquivalentTo(mappedCheckInViewModels);
        model.StaffList.Should().BeEquivalentTo(mappedStaffViewModels);
    }

    [Fact]
    public async Task List_ShouldReturnErrorView_WhenCheckInsApiFails()
    {
        // Arrange
        const string expectedViewName = "Error";
        const string errorMessage = "API call failed";

        // check-in API returns success
        _apiClientMock
            .Setup(api => api.GetAsync<List<CheckInFullDto>>(It.Is<string>(s => s.StartsWith("checkin/all?startDate="))))
            .ReturnsAsync(Result<List<CheckInFullDto>>.Failure(errorMessage));

        // Staff call should never happen because the first one fails
        _apiClientMock
            .Setup(api => api.GetAsync<List<StaffBasicDto>>(It.IsAny<string>()))
            .Verifiable();

        // Act
        var result = await _sut.List();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        var model = viewResult.Model.Should().BeOfType<ErrorViewModel>().Subject;
        model.Message.Should().Be(errorMessage);

        // Staff API must not be called after the first one fails
        _apiClientMock.Verify(api => api.GetAsync<List<StaffBasicDto>>(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task List_ShouldReturnErrorView_WhenStaffApiFails()
    {
        // Arrange
        const string expectedViewName = "Error";
        const string errorMessage = "Failed to load staff list";

        // check-in API returns success
        _apiClientMock
            .Setup(api => api.GetAsync<List<CheckInFullDto>>(It.Is<string>(s => s.StartsWith("checkin/all?startDate="))))
            .ReturnsAsync(Result<List<CheckInFullDto>>.Success(new List<CheckInFullDto>()));

        // staff API returns failure
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
    }

    [Fact]
    public async Task ListPost_ShouldReturnListViewWithCheckInDisplayAdminViewModel()
    {
        // Arrange
        const string expectedViewName = "List";

        var vm = new CheckInDisplayAdminViewModel
        {
            SelectedStaffId = "0", // "All Staff" selected
            StartDate = DateTime.Today.AddDays(-1),
            EndDate = DateTime.Today
        };

        // API returns same DTOs as above (for "all" branch)
        var checkInDtos = new List<CheckInFullDto>
        {
            new CheckInFullDto { Id = 2, FirstName = "Alice", LastName = "Smith", EmailAddress = "alice@example.com", Title = "HR", StaffId = 2, CheckInDate = DateTime.Today }
        };

        var staffDtos = new List<StaffBasicDto>
        {
            new StaffBasicDto { Id = 2, FirstName = "Alice", LastName = "Smith", EmailAddress = "alice@example.com", Title = "HR", DepartmentId = 2 }
        };

        // Controller will call checkin/all?startDate=... for SelectedStaffId == "0"
        _apiClientMock
            .Setup(api => api.GetAsync<List<CheckInFullDto>>(It.Is<string>(s => s.StartsWith("checkin/all?startDate="))))
            .ReturnsAsync(Result<List<CheckInFullDto>>.Success(checkInDtos));

        _apiClientMock
            .Setup(api => api.GetAsync<List<StaffBasicDto>>("staff/basic"))
            .ReturnsAsync(Result<List<StaffBasicDto>>.Success(staffDtos));

        var mappedCheckIns = new List<CheckInFullViewModel>
        {
            new CheckInFullViewModel { Id = 2, FirstName = "Alice", LastName = "Smith", EmailAddress = "alice@example.com", Title = "HR" }
        };
        _mapperMock
            .Setup(m => m.Map<List<CheckInFullViewModel>>(checkInDtos))
            .Returns(mappedCheckIns);

        var mappedStaff = new List<StaffBasicViewModel>
        {
            new StaffBasicViewModel { Id = 2, FirstName = "Alice", LastName = "Smith", EmailAddress = "alice@example.com", Title = "HR", DepartmentId = 2 }
        };
        _mapperMock
            .Setup(m => m.Map<List<StaffBasicViewModel>>(staffDtos))
            .Returns(mappedStaff);

        _staffSelectListServiceMock
            .Setup(s => s.GetStaffSelectListAsync(It.IsAny<CheckInDisplayAdminViewModel>(), "All Staff"))
            .ReturnsAsync(new SelectList(new List<StaffBasicViewModel>(), "Id", "FullName"));

        // Act
        IActionResult result = await _sut.List(vm);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);

        var model = viewResult.Model.Should().BeOfType<CheckInDisplayAdminViewModel>().Subject;
        model.CheckIns.Should().BeEquivalentTo(mappedCheckIns);
        model.StaffList.Should().BeEquivalentTo(mappedStaff);
    }

    [Fact]
    public async Task ListPost_ShouldCallAllCheckInsEndpoint_WhenAllStaffIsSelected()
    {
        // Arrange
        var vm = new CheckInDisplayAdminViewModel
        {
            SelectedStaffId = "0", // All Staff selected
            StartDate = DateTime.Today.AddDays(-1),
            EndDate = DateTime.Today
        };

        // Mock API call for "checkin/all" endpoint
        var checkInDtos = new List<CheckInFullDto>
        {
        new CheckInFullDto { Id = 1, FirstName = "John", LastName = "Doe", StaffId = 1 }
        };

        _apiClientMock
            .Setup(api => api.GetAsync<List<CheckInFullDto>>(It.Is<string>(s => s.StartsWith("checkin/all?startDate="))))
            .ReturnsAsync(Result<List<CheckInFullDto>>.Success(checkInDtos))
            .Verifiable(); // ensures it gets called

        // Mock staff fetch
        _apiClientMock
            .Setup(api => api.GetAsync<List<StaffBasicDto>>("staff/basic"))
            .ReturnsAsync(Result<List<StaffBasicDto>>.Success(new List<StaffBasicDto>()));

        _mapperMock.Setup(m => m.Map<List<CheckInFullViewModel>>(checkInDtos))
            .Returns(new List<CheckInFullViewModel>());

        _mapperMock.Setup(m => m.Map<List<StaffBasicViewModel>>(It.IsAny<List<StaffBasicDto>>()))
            .Returns(new List<StaffBasicViewModel>());

        _staffSelectListServiceMock
            .Setup(s => s.GetStaffSelectListAsync(It.IsAny<CheckInDisplayAdminViewModel>(), "All Staff"))
            .ReturnsAsync(new SelectList(new List<StaffBasicViewModel>(), "Id", "FullName"));

        // Act
        var result = await _sut.List(vm);

        // Assert
        _apiClientMock.Verify(api =>
            api.GetAsync<List<CheckInFullDto>>(It.Is<string>(s => s.StartsWith("checkin/all?startDate="))),
            Times.Once);
    }

    [Fact]
    public async Task ListPost_ShouldCallCheckInsByIdEndpoint_WhenSingleStaffIsSelected()
    {
        // Arrange
        var vm = new CheckInDisplayAdminViewModel
        {
            SelectedStaffId = "5", // Single staff selected
            StartDate = DateTime.Today.AddDays(-1),
            EndDate = DateTime.Today
        };

        // Mock API call for "checkin/byId/5" endpoint
        var checkInDtos = new List<CheckInFullDto>
        {
        new CheckInFullDto { Id = 2, FirstName = "Alice", LastName = "Smith", StaffId = 5 }
        };

        _apiClientMock
            .Setup(api => api.GetAsync<List<CheckInFullDto>>(It.Is<string>(s =>
                s.StartsWith("checkin/byId/5?startDate="))))
            .ReturnsAsync(Result<List<CheckInFullDto>>.Success(checkInDtos))
            .Verifiable();

        // Mock staff fetch
        _apiClientMock
            .Setup(api => api.GetAsync<List<StaffBasicDto>>("staff/basic"))
            .ReturnsAsync(Result<List<StaffBasicDto>>.Success(new List<StaffBasicDto>()));

        _mapperMock.Setup(m => m.Map<List<CheckInFullViewModel>>(checkInDtos))
            .Returns(new List<CheckInFullViewModel>());

        _mapperMock.Setup(m => m.Map<List<StaffBasicViewModel>>(It.IsAny<List<StaffBasicDto>>()))
            .Returns(new List<StaffBasicViewModel>());

        _staffSelectListServiceMock
            .Setup(s => s.GetStaffSelectListAsync(It.IsAny<CheckInDisplayAdminViewModel>(), "All Staff"))
            .ReturnsAsync(new SelectList(new List<StaffBasicViewModel>(), "Id", "FullName"));

        // Act
        var result = await _sut.List(vm);

        // Assert
        _apiClientMock.Verify(api =>
            api.GetAsync<List<CheckInFullDto>>(It.Is<string>(s =>
                s.StartsWith("checkin/byId/5?startDate="))),
            Times.Once);
    }

    [Fact]
    public async Task ListPost_ShouldReturnErrorView_WhenCheckInsApiFails()
    {
        // Arrange
        const string expectedViewName = "Error";
        const string errorMessage = "Failed to load check-ins";

        var vm = new CheckInDisplayAdminViewModel
        {
            SelectedStaffId = "0",
            StartDate = DateTime.Today.AddDays(-1),
            EndDate = DateTime.Today
        };

        _apiClientMock
            .Setup(api => api.GetAsync<List<CheckInFullDto>>(It.Is<string>(s => s.StartsWith("checkin/all?startDate="))))
            .ReturnsAsync(Result<List<CheckInFullDto>>.Failure(errorMessage));

        // Act
        var result = await _sut.List(vm);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        var model = viewResult.Model.Should().BeOfType<ErrorViewModel>().Subject;
        model.Message.Should().Be(errorMessage);
    }

    [Fact]
    public async Task Display_ShouldReturnDisplayViewWithCheckInDisplayStaffViewModel()
    {
        // Arrange
        string expectedViewName = "Display";
        string userEmail = "john.doe@johndoe.com";
        _userContextMock.Setup(x => x.GetUserEmail()).Returns(userEmail);

        var checkInDtos = new List<CheckInFullDto>
        {
        new CheckInFullDto { Id = 1, FirstName = "John", LastName = "Doe", EmailAddress = userEmail }
        };

        var mappedViewModels = new List<CheckInFullViewModel>
        {
        new CheckInFullViewModel { Id = 1, FirstName = "John", LastName = "Doe", EmailAddress = userEmail }
        };

        _apiClientMock
            .Setup(api => api.GetAsync<List<CheckInFullDto>>(It.Is<string>(s =>
                s.StartsWith("checkin/byEmail?emailAddress="))))
            .ReturnsAsync(Result<List<CheckInFullDto>>.Success(checkInDtos));

        _mapperMock
            .Setup(m => m.Map<List<CheckInFullViewModel>>(checkInDtos))
            .Returns(mappedViewModels);

        // Act
        IActionResult result = await _sut.Display();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        var model = viewResult.Model.Should().BeOfType<CheckInDisplayStaffViewModel>().Subject;
        model.CheckIns.Should().BeEquivalentTo(mappedViewModels);
    }

    [Fact]
    public async Task Display_ShouldReturnErrorView_WhenApiCallFails()
    {
        // Arrange
        const string expectedViewName = "Error";
        const string errorMessage = "Failed to load check-ins";

        string userEmail = "john.doe@johndoe.com";
        _userContextMock.Setup(x => x.GetUserEmail()).Returns(userEmail);

        _apiClientMock
            .Setup(api => api.GetAsync<List<CheckInFullDto>>(It.Is<string>(s =>
                s.StartsWith("checkin/byEmail?emailAddress="))))
            .ReturnsAsync(Result<List<CheckInFullDto>>.Failure(errorMessage));

        // Act
        var result = await _sut.Display();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        var model = viewResult.Model.Should().BeOfType<ErrorViewModel>().Subject;
        model.Message.Should().Be(errorMessage);

        // Mapper should never be called
        _mapperMock.Verify(m => m.Map<List<CheckInFullViewModel>>(It.IsAny<List<CheckInFullDto>>()), Times.Never);
    }

    [Fact]
    public async Task DisplayPost_ShouldReturnDisplayViewWithCheckInDisplayStaffViewModel()
    {
        // Arrange
        string expectedViewName = "Display";
        string userEmail = "john.doe@johndoe.com";
        _userContextMock.Setup(x => x.GetUserEmail()).Returns(userEmail);

        var inputModel = new CheckInDisplayStaffViewModel
        {
            StartDate = DateTime.Today.AddDays(-3),
            EndDate = DateTime.Today
        };

        var checkInDtos = new List<CheckInFullDto>
        {
        new CheckInFullDto { Id = 2, FirstName = "Alice", LastName = "Smith", EmailAddress = userEmail }
        };

        var mappedViewModels = new List<CheckInFullViewModel>
        {
        new CheckInFullViewModel { Id = 2, FirstName = "Alice", LastName = "Smith", EmailAddress = userEmail  }
        };

        _apiClientMock
            .Setup(api => api.GetAsync<List<CheckInFullDto>>(It.Is<string>(s =>
                s.Contains("checkin/byEmail") && s.Contains(Uri.EscapeDataString(userEmail)))))
            .ReturnsAsync(Result<List<CheckInFullDto>>.Success(checkInDtos));

        _mapperMock
            .Setup(m => m.Map<List<CheckInFullViewModel>>(checkInDtos))
            .Returns(mappedViewModels);

        // Act
        IActionResult result = await _sut.Display(inputModel);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);
        var model = viewResult.Model.Should().BeOfType<CheckInDisplayStaffViewModel>().Subject;
        model.CheckIns.Should().BeEquivalentTo(mappedViewModels);
    }

    [Fact]
    public async Task DisplayPost_ShouldReturnErrorView_WhenApiCallFails()
    {
        // Arrange
        const string expectedViewName = "Error";
        const string errorMessage = "API request failed";

        string userEmail = "john.doe@johndoe.com";
        _userContextMock.Setup(x => x.GetUserEmail()).Returns(userEmail);

        var inputModel = new CheckInDisplayStaffViewModel
        {
            StartDate = DateTime.Today.AddDays(-7),
            EndDate = DateTime.Today
        };

        _apiClientMock
            .Setup(api => api.GetAsync<List<CheckInFullDto>>(It.Is<string>(s =>
                s.Contains("checkin/byEmail") && s.Contains(Uri.EscapeDataString(userEmail)))))
            .ReturnsAsync(Result<List<CheckInFullDto>>.Failure(errorMessage));

        // Act
        var result = await _sut.Display(inputModel);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.ViewName.Should().Be(expectedViewName);

        var model = viewResult.Model.Should().BeOfType<ErrorViewModel>().Subject;
        model.Message.Should().Be(errorMessage);

        // Mapper should never be called
        _mapperMock.Verify(m => m.Map<List<CheckInFullViewModel>>(It.IsAny<List<CheckInFullDto>>()), Times.Never);
    }

}
