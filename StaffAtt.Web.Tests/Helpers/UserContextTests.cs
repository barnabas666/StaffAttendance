using Moq;
using FluentAssertions;
using StaffAtt.Web.Helpers;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace StaffAtt.Web.Tests.Helpers;
public class UserContextTests
{
    private readonly UserContext _sut;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsPrincipalMock = new();

    public UserContextTests()
    {
        _httpContextAccessorMock.Setup(x => x.HttpContext.User).Returns(_claimsPrincipalMock.Object);
        _sut = new UserContext(_httpContextAccessorMock.Object);
    }

    [Fact]
    public void GetUserEmail_ShouldReturnEmailFromClaims()
    {
        // Arrange
        string expectedEmail = "john.doe@johndoe.com";
        _claimsPrincipalMock.Setup(x => x.FindFirst(ClaimTypes.Email))
            .Returns(new Claim(ClaimTypes.Email, expectedEmail));
        // Act
        var result = _sut.GetUserEmail();
        // Assert
        result.Should().Be(expectedEmail);
    }
}
