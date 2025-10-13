using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Fadebook.Controllers;
using Fadebook.Models;
using Fadebook.Services;
using Fadebook.Exceptions;

namespace Api.Tests.Controllers;

public class BarberWorkHoursControllerTests
{
    private readonly Mock<IBarberWorkHoursService> _mockService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly BarberWorkHoursController _controller;

    public BarberWorkHoursControllerTests()
    {
        _mockService = new Mock<IBarberWorkHoursService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new BarberWorkHoursController(_mockService.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAllWorkHours_ReturnsOk_WithAllWorkHours()
    {
        // Arrange
        var workHours = new List<BarberWorkHoursModel>
        {
            new BarberWorkHoursModel
            {
                WorkHourId = 1,
                BarberId = 1,
                DayOfWeek = 1,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0),
                IsActive = true,
                Barber = new BarberModel { BarberId = 1, Name = "John Barber" }
            }
        };
        var workHoursDtos = new List<BarberWorkHoursDto>
        {
            new BarberWorkHoursDto
            {
                WorkHourId = 1,
                BarberId = 1,
                BarberName = "John Barber",
                DayOfWeek = 1,
                DayOfWeekName = "Monday",
                StartTime = "09:00",
                EndTime = "17:00",
                IsActive = true
            }
        };

        _mockService.Setup(s => s.GetAllWorkHoursAsync()).ReturnsAsync(workHours);
        _mockMapper.Setup(m => m.Map<IEnumerable<BarberWorkHoursDto>>(workHours)).Returns(workHoursDtos);

        // Act
        var result = await _controller.GetAllWorkHours();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(workHoursDtos);
        _mockService.Verify(s => s.GetAllWorkHoursAsync(), Times.Once);
    }

    [Fact]
    public async Task GetWorkHoursById_ReturnsOk_WhenWorkHoursExist()
    {
        // Arrange
        var workHours = new BarberWorkHoursModel
        {
            WorkHourId = 1,
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true,
            Barber = new BarberModel { BarberId = 1, Name = "John Barber" }
        };
        var workHoursDto = new BarberWorkHoursDto
        {
            WorkHourId = 1,
            BarberId = 1,
            BarberName = "John Barber",
            DayOfWeek = 1,
            DayOfWeekName = "Monday",
            StartTime = "09:00",
            EndTime = "17:00",
            IsActive = true
        };

        _mockService.Setup(s => s.GetWorkHoursByIdAsync(1)).ReturnsAsync(workHours);
        _mockMapper.Setup(m => m.Map<BarberWorkHoursDto>(workHours)).Returns(workHoursDto);

        // Act
        var result = await _controller.GetWorkHoursById(1);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(workHoursDto);
        _mockService.Verify(s => s.GetWorkHoursByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetWorkHoursById_ThrowsNotFound_WhenWorkHoursDoNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.GetWorkHoursByIdAsync(999)).ThrowsAsync(new NotFoundException("Work hours not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetWorkHoursById(999));
    }

    [Fact]
    public async Task GetWorkHoursByBarberId_ReturnsOk_WithWorkHours()
    {
        // Arrange
        var workHours = new List<BarberWorkHoursModel>
        {
            new BarberWorkHoursModel
            {
                WorkHourId = 1,
                BarberId = 1,
                DayOfWeek = 1,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0),
                IsActive = true,
                Barber = new BarberModel { BarberId = 1, Name = "John Barber" }
            }
        };
        var workHoursDtos = new List<BarberWorkHoursDto>
        {
            new BarberWorkHoursDto
            {
                WorkHourId = 1,
                BarberId = 1,
                BarberName = "John Barber",
                DayOfWeek = 1,
                DayOfWeekName = "Monday",
                StartTime = "09:00",
                EndTime = "17:00",
                IsActive = true
            }
        };

        _mockService.Setup(s => s.GetWorkHoursByBarberIdAsync(1)).ReturnsAsync(workHours);
        _mockMapper.Setup(m => m.Map<IEnumerable<BarberWorkHoursDto>>(workHours)).Returns(workHoursDtos);

        // Act
        var result = await _controller.GetWorkHoursByBarberId(1);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(workHoursDtos);
    }

    [Fact]
    public async Task GetWorkHoursByBarberId_ThrowsNotFound_WhenBarberDoesNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.GetWorkHoursByBarberIdAsync(999)).ThrowsAsync(new NotFoundException("Barber not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetWorkHoursByBarberId(999));
    }

    [Fact]
    public async Task GetWorkHoursByBarberIdAndDay_ReturnsOk_WithWorkHours()
    {
        // Arrange
        var workHours = new List<BarberWorkHoursModel>
        {
            new BarberWorkHoursModel
            {
                WorkHourId = 1,
                BarberId = 1,
                DayOfWeek = 1,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0),
                IsActive = true,
                Barber = new BarberModel { BarberId = 1, Name = "John Barber" }
            }
        };
        var workHoursDtos = new List<BarberWorkHoursDto>
        {
            new BarberWorkHoursDto
            {
                WorkHourId = 1,
                BarberId = 1,
                BarberName = "John Barber",
                DayOfWeek = 1,
                DayOfWeekName = "Monday",
                StartTime = "09:00",
                EndTime = "17:00",
                IsActive = true
            }
        };

        _mockService.Setup(s => s.GetWorkHoursByBarberIdAndDayAsync(1, 1)).ReturnsAsync(workHours);
        _mockMapper.Setup(m => m.Map<IEnumerable<BarberWorkHoursDto>>(workHours)).Returns(workHoursDtos);

        // Act
        var result = await _controller.GetWorkHoursByBarberIdAndDay(1, 1);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(workHoursDtos);
    }

    [Fact]
    public async Task IsBarberAvailable_ReturnsOk_WhenBarberIsAvailable()
    {
        // Arrange
        var appointmentDateTime = new DateTime(2025, 10, 13, 10, 0, 0);
        _mockService.Setup(s => s.IsBarberAvailableAsync(1, appointmentDateTime, 30)).ReturnsAsync(true);

        // Act
        var result = await _controller.IsBarberAvailable(1, appointmentDateTime, 30);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task IsBarberAvailable_ReturnsOk_WhenBarberIsNotAvailable()
    {
        // Arrange
        var appointmentDateTime = new DateTime(2025, 10, 13, 10, 0, 0);
        _mockService.Setup(s => s.IsBarberAvailableAsync(1, appointmentDateTime, 30)).ReturnsAsync(false);

        // Act
        var result = await _controller.IsBarberAvailable(1, appointmentDateTime, 30);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAvailableTimeSlots_ReturnsOk_WithTimeSlots()
    {
        // Arrange
        var date = "2025-10-13";
        var parsedDate = new DateTime(2025, 10, 13);
        var slots = new List<DateTime>
        {
            new DateTime(2025, 10, 13, 9, 0, 0, DateTimeKind.Unspecified),
            new DateTime(2025, 10, 13, 9, 30, 0, DateTimeKind.Unspecified),
            new DateTime(2025, 10, 13, 10, 0, 0, DateTimeKind.Unspecified)
        };
        var expectedSlotStrings = new List<string>
        {
            "2025-10-13T09:00:00",
            "2025-10-13T09:30:00",
            "2025-10-13T10:00:00"
        };

        _mockService.Setup(s => s.GetAvailableTimeSlotsAsync(1, parsedDate, 30)).ReturnsAsync(slots);

        // Act
        var result = await _controller.GetAvailableTimeSlots(1, date, 30);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(expectedSlotStrings);
    }

    [Fact]
    public async Task GetAvailableTimeSlots_ReturnsBadRequest_WhenDateFormatInvalid()
    {
        // Arrange
        var invalidDate = "invalid-date";

        // Act
        var result = await _controller.GetAvailableTimeSlots(1, invalidDate, 30);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Value.Should().Be("Invalid date format. Use YYYY-MM-DD.");
    }

    [Fact]
    public async Task AddWorkHours_ReturnsCreatedAtAction_WhenValid()
    {
        // Arrange
        var createDto = new CreateBarberWorkHoursDto
        {
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = "09:00",
            EndTime = "17:00",
            IsActive = true
        };
        var workHours = new BarberWorkHoursModel
        {
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };
        var createdWorkHours = new BarberWorkHoursModel
        {
            WorkHourId = 1,
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true,
            Barber = new BarberModel { BarberId = 1, Name = "John Barber" }
        };
        var resultDto = new BarberWorkHoursDto
        {
            WorkHourId = 1,
            BarberId = 1,
            BarberName = "John Barber",
            DayOfWeek = 1,
            DayOfWeekName = "Monday",
            StartTime = "09:00",
            EndTime = "17:00",
            IsActive = true
        };

        _mockMapper.Setup(m => m.Map<BarberWorkHoursModel>(createDto)).Returns(workHours);
        _mockService.Setup(s => s.AddWorkHoursAsync(workHours)).ReturnsAsync(createdWorkHours);
        _mockMapper.Setup(m => m.Map<BarberWorkHoursDto>(createdWorkHours)).Returns(resultDto);

        // Act
        var result = await _controller.AddWorkHours(createDto);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult.ActionName.Should().Be(nameof(_controller.GetWorkHoursById));
        createdResult.RouteValues["workHourId"].Should().Be(1);
        createdResult.Value.Should().BeEquivalentTo(resultDto);
    }

    [Fact]
    public async Task AddWorkHours_ThrowsNotFoundException_WhenBarberDoesNotExist()
    {
        // Arrange
        var createDto = new CreateBarberWorkHoursDto
        {
            BarberId = 999,
            DayOfWeek = 1,
            StartTime = "09:00",
            EndTime = "17:00",
            IsActive = true
        };
        var workHours = new BarberWorkHoursModel
        {
            BarberId = 999,
            DayOfWeek = 1,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };

        _mockMapper.Setup(m => m.Map<BarberWorkHoursModel>(createDto)).Returns(workHours);
        _mockService.Setup(s => s.AddWorkHoursAsync(workHours)).ThrowsAsync(new NotFoundException("Barber not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.AddWorkHours(createDto));
    }

    [Fact]
    public async Task AddWorkHours_ThrowsBadRequest_WhenStartTimeAfterEndTime()
    {
        // Arrange
        var createDto = new CreateBarberWorkHoursDto
        {
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = "17:00",
            EndTime = "09:00",
            IsActive = true
        };
        var workHours = new BarberWorkHoursModel
        {
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(17, 0),
            EndTime = new TimeOnly(9, 0),
            IsActive = true
        };

        _mockMapper.Setup(m => m.Map<BarberWorkHoursModel>(createDto)).Returns(workHours);
        _mockService.Setup(s => s.AddWorkHoursAsync(workHours)).ThrowsAsync(new BadRequestException("Start time must be before end time"));

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _controller.AddWorkHours(createDto));
    }

    [Fact]
    public async Task UpdateWorkHours_ReturnsOk_WhenValid()
    {
        // Arrange
        var updateDto = new CreateBarberWorkHoursDto
        {
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = "10:00",
            EndTime = "18:00",
            IsActive = true
        };
        var workHours = new BarberWorkHoursModel
        {
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(18, 0),
            IsActive = true
        };
        var updatedWorkHours = new BarberWorkHoursModel
        {
            WorkHourId = 1,
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(18, 0),
            IsActive = true,
            Barber = new BarberModel { BarberId = 1, Name = "John Barber" }
        };
        var resultDto = new BarberWorkHoursDto
        {
            WorkHourId = 1,
            BarberId = 1,
            BarberName = "John Barber",
            DayOfWeek = 1,
            DayOfWeekName = "Monday",
            StartTime = "10:00",
            EndTime = "18:00",
            IsActive = true
        };

        _mockMapper.Setup(m => m.Map<BarberWorkHoursModel>(updateDto)).Returns(workHours);
        _mockService.Setup(s => s.UpdateWorkHoursAsync(1, workHours)).ReturnsAsync(updatedWorkHours);
        _mockMapper.Setup(m => m.Map<BarberWorkHoursDto>(updatedWorkHours)).Returns(resultDto);

        // Act
        var result = await _controller.UpdateWorkHours(1, updateDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(resultDto);
    }

    [Fact]
    public async Task UpdateWorkHours_ThrowsNotFound_WhenWorkHoursDoNotExist()
    {
        // Arrange
        var updateDto = new CreateBarberWorkHoursDto
        {
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = "10:00",
            EndTime = "18:00",
            IsActive = true
        };
        var workHours = new BarberWorkHoursModel
        {
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(18, 0),
            IsActive = true
        };

        _mockMapper.Setup(m => m.Map<BarberWorkHoursModel>(updateDto)).Returns(workHours);
        _mockService.Setup(s => s.UpdateWorkHoursAsync(999, workHours)).ThrowsAsync(new NotFoundException("Work hours not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.UpdateWorkHours(999, updateDto));
    }

    [Fact]
    public async Task DeleteWorkHours_ReturnsOk_WhenWorkHoursDeleted()
    {
        // Arrange
        var deletedWorkHours = new BarberWorkHoursModel
        {
            WorkHourId = 1,
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true,
            Barber = new BarberModel { BarberId = 1, Name = "John Barber" }
        };
        var resultDto = new BarberWorkHoursDto
        {
            WorkHourId = 1,
            BarberId = 1,
            BarberName = "John Barber",
            DayOfWeek = 1,
            DayOfWeekName = "Monday",
            StartTime = "09:00",
            EndTime = "17:00",
            IsActive = true
        };

        _mockService.Setup(s => s.DeleteWorkHoursAsync(1)).ReturnsAsync(deletedWorkHours);
        _mockMapper.Setup(m => m.Map<BarberWorkHoursDto>(deletedWorkHours)).Returns(resultDto);

        // Act
        var result = await _controller.DeleteWorkHours(1);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(resultDto);
    }

    [Fact]
    public async Task DeleteWorkHours_ThrowsNotFound_WhenWorkHoursDoNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteWorkHoursAsync(999)).ThrowsAsync(new NotFoundException("Work hours not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.DeleteWorkHours(999));
    }
}
