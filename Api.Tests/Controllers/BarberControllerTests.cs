using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Fadebook.Controllers;
using Fadebook.Models;
using Fadebook.Services;
using Fadebook.DTOs;

namespace Api.Tests.Controllers;

public class BarberControllerTests
{
    private readonly Mock<IBarberManagementService> _mockService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly BarberController _controller;

    public BarberControllerTests()
    {
        _mockService = new Mock<IBarberManagementService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new BarberController(_mockService.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithBarbers()
    {
        // Arrange
        var barbers = new List<BarberModel>
        {
            new BarberModel { BarberId = 1, Username = "barber1", Name = "John Barber" },
            new BarberModel { BarberId = 2, Username = "barber2", Name = "Jane Barber" }
        };
        var barberDtos = new List<BarberDto>
        {
            new BarberDto { BarberId = 1, Username = "barber1", Name = "John Barber" },
            new BarberDto { BarberId = 2, Username = "barber2", Name = "Jane Barber" }
        };

        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(barbers);
        _mockMapper.Setup(m => m.Map<IEnumerable<BarberDto>>(barbers)).Returns(barberDtos);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(barberDtos);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenBarberExists()
    {
        // Arrange
        var barberModel = new BarberModel { BarberId = 1, Username = "barber1", Name = "John Barber" };
        var barberDto = new BarberDto { BarberId = 1, Username = "barber1", Name = "John Barber" };

        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(barberModel);
        _mockMapper.Setup(m => m.Map<BarberDto>(barberModel)).Returns(barberDto);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(barberDto);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenBarberDoesNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((BarberModel)null);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenBarberIsValid()
    {
        // Arrange
        var barberDto = new BarberDto { Username = "barber3", Name = "Bob Barber" };
        var barberModel = new BarberModel { Username = "barber3", Name = "Bob Barber" };
        var createdModel = new BarberModel { BarberId = 3, Username = "barber3", Name = "Bob Barber" };

        _mockMapper.Setup(m => m.Map<BarberModel>(barberDto)).Returns(barberModel);
        _mockService.Setup(s => s.AddAsync(barberModel)).ReturnsAsync(createdModel);
        _mockMapper.Setup(m => m.Map<BarberDto>(createdModel)).Returns(barberDto);

        // Act
        var result = await _controller.Create(barberDto);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
        var createdResult = result.Result as CreatedResult;
        createdResult.Location.Should().Be("api/barber/3");
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenBarberIsUpdated()
    {
        // Arrange
        var barberDto = new BarberDto { Username = "barber1", Name = "John Updated" };
        var barberModel = new BarberModel { BarberId = 1, Username = "barber1", Name = "John Updated" };

        _mockMapper.Setup(m => m.Map<BarberModel>(barberDto)).Returns(barberModel);
        _mockService.Setup(s => s.UpdateAsync(1, It.IsAny<BarberModel>())).ReturnsAsync(barberModel);
        _mockMapper.Setup(m => m.Map<BarberDto>(barberModel)).Returns(barberDto);

        // Act
        var result = await _controller.Update(1, barberDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenBarberDoesNotExist()
    {
        // Arrange
        var barberDto = new BarberDto();
        var barberModel = new BarberModel();

        _mockMapper.Setup(m => m.Map<BarberModel>(barberDto)).Returns(barberModel);
        _mockService.Setup(s => s.UpdateAsync(1, It.IsAny<BarberModel>())).ReturnsAsync((BarberModel)null);

        // Act
        var result = await _controller.Update(1, barberDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenBarberIsDeleted()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteByIdAsync(1)).ReturnsAsync((BarberModel)null);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenBarberDoesNotExist()
    {
        // Arrange
        var barberModel = new BarberModel { BarberId = 1 };
        _mockService.Setup(s => s.DeleteByIdAsync(1)).ReturnsAsync(barberModel);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateServices_ReturnsNoContent_WhenServicesAreUpdated()
    {
        // Arrange
        var serviceIds = new List<int> { 1, 2, 3 };
        _mockService.Setup(s => s.UpdateBarberServicesAsync(1, serviceIds)).ReturnsAsync((IEnumerable<BarberServiceModel>)null);

        // Act
        var result = await _controller.UpdateServices(1, serviceIds);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateServices_ReturnsBadRequest_WhenServiceIdsAreEmpty()
    {
        // Arrange
        var serviceIds = new List<int>();

        // Act
        var result = await _controller.UpdateServices(1, serviceIds);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateServices_ReturnsNotFound_WhenBarberDoesNotExist()
    {
        // Arrange
        var serviceIds = new List<int> { 1, 2, 3 };
        var barberServices = new List<BarberServiceModel>();
        _mockService.Setup(s => s.UpdateBarberServicesAsync(1, serviceIds)).ReturnsAsync(barberServices);

        // Act
        var result = await _controller.UpdateServices(1, serviceIds);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
