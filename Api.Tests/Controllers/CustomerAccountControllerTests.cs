using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Fadebook.Controllers;
using Fadebook.Services;
using Fadebook.DTOs;

namespace Api.Tests.Controllers;

public class CustomerAccountControllerTests
{
    private readonly Mock<ICustomerAppointmentService> _mockCustomerAppointmentService;
    private readonly Mock<IUserAccountService> _mockUserAccountService;
    private readonly CustomerAccountController _controller;

    public CustomerAccountControllerTests()
    {
        _mockCustomerAppointmentService = new Mock<ICustomerAppointmentService>();
        _mockUserAccountService = new Mock<IUserAccountService>();
        _controller = new CustomerAccountController(
            _mockCustomerAppointmentService.Object,
            _mockUserAccountService.Object
        );
    }

    [Fact]
    public async Task CheckUsername_ReturnsOk_WithTrue_WhenUsernameExists()
    {
        // Arrange
        var nameDto = new NameDTO { Name = "existinguser" };
        _mockUserAccountService.Setup(s => s.CheckIfUsernameExistsAsync("existinguser")).ReturnsAsync(true);

        // Act
        var result = await _controller.CheckUsername(nameDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var isTakenDto = okResult.Value as IsTakenDTO;
        isTakenDto.IsTaken.Should().BeTrue();
    }

    [Fact]
    public async Task CheckUsername_ReturnsOk_WithFalse_WhenUsernameDoesNotExist()
    {
        // Arrange
        var nameDto = new NameDTO { Name = "newuser" };
        _mockUserAccountService.Setup(s => s.CheckIfUsernameExistsAsync("newuser")).ReturnsAsync(false);

        // Act
        var result = await _controller.CheckUsername(nameDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var isTakenDto = okResult.Value as IsTakenDTO;
        isTakenDto.IsTaken.Should().BeFalse();
    }

    [Fact]
    public async Task CheckUsername_CallsServiceWithCorrectUsername()
    {
        // Arrange
        var nameDto = new NameDTO { Name = "testuser" };
        _mockUserAccountService.Setup(s => s.CheckIfUsernameExistsAsync("testuser")).ReturnsAsync(false);

        // Act
        await _controller.CheckUsername(nameDto);

        // Assert
        _mockUserAccountService.Verify(s => s.CheckIfUsernameExistsAsync("testuser"), Times.Once);
    }
}
