using FluentAssertions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StaffAtt.Web.Controllers;
using StaffAtt.Web.Models;
using StaffAttLibrary.Data;
using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Security.Claims;

namespace StaffAtt.Web.Tests.Controllers;
public class StaffControllerTests
{
    private readonly StaffController _sut;
    private readonly Mock<IStaffService> _staffServiceMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IPrincipal> _claimsPrincipalMock = new();

    public StaffControllerTests()
    {
        _sut = new StaffController(_staffServiceMock.Object, null, null, _mapperMock.Object);
    }

    [Fact]
    public async Task Create_ShouldReturnSuccess()
    {
        // Arrange
        var departments = new List<DepartmentModel>
        {
            new DepartmentModel { Id = 1, Title = "IT", Description = "IT department." },
            new DepartmentModel { Id = 2, Title = "HR", Description = "Human resources department." }
        };
        _staffServiceMock.Setup(x => x.GetAllDepartmentsAsync())
            .ReturnsAsync(departments);
        // Act
        IActionResult result = await _sut.Create();
        // Assert
        ViewResult viewResult = result.Should().BeOfType<ViewResult>().Subject;
        StaffCreateViewModel model = viewResult.Model.Should().BeAssignableTo<StaffCreateViewModel>().Subject;
        model.DepartmentItems.Should().NotBeNull();
    }
}
