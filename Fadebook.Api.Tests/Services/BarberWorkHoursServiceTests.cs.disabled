using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Fadebook.Models;
using Fadebook.Repositories;
using Fadebook.Services;
using Fadebook.Exceptions;

namespace Api.Tests.Services;

public class BarberWorkHoursServiceTests
{
    private readonly Mock<IDbTransactionContext> _mockDbTransactionContext;
    private readonly Mock<IBarberWorkHoursRepository> _mockWorkHoursRepository;
    private readonly Mock<IBarberRepository> _mockBarberRepository;
    private readonly Mock<IAppointmentRepository> _mockAppointmentRepository;
    private readonly BarberWorkHoursService _service;

    public BarberWorkHoursServiceTests()
    {
        _mockDbTransactionContext = new Mock<IDbTransactionContext>();
        _mockWorkHoursRepository = new Mock<IBarberWorkHoursRepository>();
        _mockBarberRepository = new Mock<IBarberRepository>();
        _mockAppointmentRepository = new Mock<IAppointmentRepository>();
        _service = new BarberWorkHoursService(
            _mockDbTransactionContext.Object,
            _mockWorkHoursRepository.Object,
            _mockBarberRepository.Object,
            _mockAppointmentRepository.Object
        );
    }

    [Fact]
    public async Task GetAllWorkHoursAsync_ShouldReturnAllWorkHours()
    {
        // Arrange
        var workHoursList = new List<BarberWorkHoursModel>
        {
            new BarberWorkHoursModel
            {
                WorkHourId = 1,
                BarberId = 1,
                DayOfWeek = 1,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0),
                IsActive = true
            },
            new BarberWorkHoursModel
            {
                WorkHourId = 2,
                BarberId = 1,
                DayOfWeek = 2,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0),
                IsActive = true
            }
        };
        _mockWorkHoursRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(workHoursList);

        // Act
        var result = await _service.GetAllWorkHoursAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(workHoursList);
        _mockWorkHoursRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetWorkHoursByIdAsync_WhenWorkHoursExist_ShouldReturnWorkHours()
    {
        // Arrange
        var workHours = new BarberWorkHoursModel
        {
            WorkHourId = 1,
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };
        _mockWorkHoursRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(workHours);

        // Act
        var result = await _service.GetWorkHoursByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(workHours);
        _mockWorkHoursRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetWorkHoursByIdAsync_WhenWorkHoursDoNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockWorkHoursRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((BarberWorkHoursModel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetWorkHoursByIdAsync(1));
    }

    [Fact]
    public async Task GetWorkHoursByBarberIdAsync_WhenBarberExists_ShouldReturnWorkHours()
    {
        // Arrange
        var barber = new BarberModel { BarberId = 1, Username = "john", Name = "John Barber" };
        var workHoursList = new List<BarberWorkHoursModel>
        {
            new BarberWorkHoursModel
            {
                WorkHourId = 1,
                BarberId = 1,
                DayOfWeek = 1,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0),
                IsActive = true
            }
        };

        _mockBarberRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(barber);
        _mockWorkHoursRepository.Setup(r => r.GetByBarberIdAsync(1)).ReturnsAsync(workHoursList);

        // Act
        var result = await _service.GetWorkHoursByBarberIdAsync(1);

        // Assert
        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo(workHoursList);
        _mockBarberRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockWorkHoursRepository.Verify(r => r.GetByBarberIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetWorkHoursByBarberIdAsync_WhenBarberDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockBarberRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((BarberModel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetWorkHoursByBarberIdAsync(1));
    }

    [Fact]
    public async Task GetWorkHoursByBarberIdAndDayAsync_WhenValid_ShouldReturnWorkHours()
    {
        // Arrange
        var barber = new BarberModel { BarberId = 1, Username = "john", Name = "John Barber" };
        var workHoursList = new List<BarberWorkHoursModel>
        {
            new BarberWorkHoursModel
            {
                WorkHourId = 1,
                BarberId = 1,
                DayOfWeek = 1,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0),
                IsActive = true
            }
        };

        _mockBarberRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(barber);
        _mockWorkHoursRepository.Setup(r => r.GetByBarberIdAndDayAsync(1, 1)).ReturnsAsync(workHoursList);

        // Act
        var result = await _service.GetWorkHoursByBarberIdAndDayAsync(1, 1);

        // Assert
        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo(workHoursList);
    }

    [Fact]
    public async Task GetWorkHoursByBarberIdAndDayAsync_WhenDayOfWeekInvalid_ShouldThrowBadRequestException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _service.GetWorkHoursByBarberIdAndDayAsync(1, 7)); // Invalid day (7)
    }

    [Fact]
    public async Task GetWorkHoursByBarberIdAndDayAsync_WhenDayOfWeekNegative_ShouldThrowBadRequestException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _service.GetWorkHoursByBarberIdAndDayAsync(1, -1)); // Invalid day (-1)
    }

    [Fact]
    public async Task AddWorkHoursAsync_WhenValid_ShouldAddAndSave()
    {
        // Arrange
        var barber = new BarberModel { BarberId = 1, Username = "john", Name = "John Barber" };
        var workHours = new BarberWorkHoursModel
        {
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };
        var addedWorkHours = new BarberWorkHoursModel
        {
            WorkHourId = 1,
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };

        _mockBarberRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(barber);
        _mockWorkHoursRepository.Setup(r => r.AddAsync(workHours)).ReturnsAsync(addedWorkHours);
        _mockDbTransactionContext.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _service.AddWorkHoursAsync(workHours);

        // Assert
        result.Should().NotBeNull();
        result.WorkHourId.Should().Be(1);
        _mockBarberRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockWorkHoursRepository.Verify(r => r.AddAsync(workHours), Times.Once);
        _mockDbTransactionContext.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddWorkHoursAsync_WhenBarberDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var workHours = new BarberWorkHoursModel
        {
            BarberId = 999,
            DayOfWeek = 1,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };

        _mockBarberRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((BarberModel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.AddWorkHoursAsync(workHours));
    }

    [Fact]
    public async Task AddWorkHoursAsync_WhenStartTimeAfterEndTime_ShouldThrowBadRequestException()
    {
        // Arrange
        var barber = new BarberModel { BarberId = 1, Username = "john", Name = "John Barber" };
        var workHours = new BarberWorkHoursModel
        {
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(17, 0), // After end time
            EndTime = new TimeOnly(9, 0),
            IsActive = true
        };

        _mockBarberRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(barber);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.AddWorkHoursAsync(workHours));
    }

    [Fact]
    public async Task AddWorkHoursAsync_WhenStartTimeEqualsEndTime_ShouldThrowBadRequestException()
    {
        // Arrange
        var barber = new BarberModel { BarberId = 1, Username = "john", Name = "John Barber" };
        var workHours = new BarberWorkHoursModel
        {
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(9, 0), // Same as start time
            IsActive = true
        };

        _mockBarberRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(barber);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.AddWorkHoursAsync(workHours));
    }

    [Fact]
    public async Task AddWorkHoursAsync_WhenDayOfWeekInvalid_ShouldThrowBadRequestException()
    {
        // Arrange
        var barber = new BarberModel { BarberId = 1, Username = "john", Name = "John Barber" };
        var workHours = new BarberWorkHoursModel
        {
            BarberId = 1,
            DayOfWeek = 7, // Invalid
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };

        _mockBarberRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(barber);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.AddWorkHoursAsync(workHours));
    }

    [Fact]
    public async Task UpdateWorkHoursAsync_WhenValid_ShouldUpdateAndSave()
    {
        // Arrange
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
            IsActive = true
        };

        _mockWorkHoursRepository.Setup(r => r.UpdateAsync(1, workHours)).ReturnsAsync(updatedWorkHours);
        _mockDbTransactionContext.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _service.UpdateWorkHoursAsync(1, workHours);

        // Assert
        result.Should().NotBeNull();
        result.WorkHourId.Should().Be(1);
        _mockWorkHoursRepository.Verify(r => r.UpdateAsync(1, workHours), Times.Once);
        _mockDbTransactionContext.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateWorkHoursAsync_WhenWorkHoursDoNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var workHours = new BarberWorkHoursModel
        {
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };

        _mockWorkHoursRepository.Setup(r => r.UpdateAsync(999, workHours)).ReturnsAsync((BarberWorkHoursModel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateWorkHoursAsync(999, workHours));
    }

    [Fact]
    public async Task UpdateWorkHoursAsync_WhenStartTimeAfterEndTime_ShouldThrowBadRequestException()
    {
        // Arrange
        var workHours = new BarberWorkHoursModel
        {
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(17, 0),
            EndTime = new TimeOnly(9, 0),
            IsActive = true
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.UpdateWorkHoursAsync(1, workHours));
    }

    [Fact]
    public async Task DeleteWorkHoursAsync_WhenWorkHoursExist_ShouldDeleteAndSave()
    {
        // Arrange
        var workHours = new BarberWorkHoursModel
        {
            WorkHourId = 1,
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };

        _mockWorkHoursRepository.Setup(r => r.RemoveByIdAsync(1)).ReturnsAsync(workHours);
        _mockDbTransactionContext.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _service.DeleteWorkHoursAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(workHours);
        _mockWorkHoursRepository.Verify(r => r.RemoveByIdAsync(1), Times.Once);
        _mockDbTransactionContext.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteWorkHoursAsync_WhenWorkHoursDoNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockWorkHoursRepository.Setup(r => r.RemoveByIdAsync(999)).ReturnsAsync((BarberWorkHoursModel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteWorkHoursAsync(999));
    }

    [Fact]
    public async Task IsBarberAvailableAsync_ShouldCallRepository()
    {
        // Arrange
        var appointmentDateTime = new DateTime(2025, 10, 13, 10, 0, 0);
        _mockWorkHoursRepository.Setup(r => r.IsBarberAvailableAsync(1, appointmentDateTime, 30)).ReturnsAsync(true);

        // Act
        var result = await _service.IsBarberAvailableAsync(1, appointmentDateTime, 30);

        // Assert
        result.Should().BeTrue();
        _mockWorkHoursRepository.Verify(r => r.IsBarberAvailableAsync(1, appointmentDateTime, 30), Times.Once);
    }

    [Fact]
    public async Task GetAvailableTimeSlotsAsync_WhenNoWorkHours_ShouldReturnEmptyList()
    {
        // Arrange
        var date = new DateTime(2025, 10, 13); // Monday
        _mockWorkHoursRepository.Setup(r => r.GetByBarberIdAndDayAsync(1, 1)).ReturnsAsync(new List<BarberWorkHoursModel>());

        // Act
        var result = await _service.GetAvailableTimeSlotsAsync(1, date, 30);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAvailableTimeSlotsAsync_WhenNoAppointments_ShouldReturnAllSlots()
    {
        // Arrange
        var date = new DateTime(2025, 10, 13); // Monday
        var workHours = new List<BarberWorkHoursModel>
        {
            new BarberWorkHoursModel
            {
                WorkHourId = 1,
                BarberId = 1,
                DayOfWeek = 1,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(11, 0), // 2 hours = 4 slots of 30 minutes
                IsActive = true
            }
        };

        _mockWorkHoursRepository.Setup(r => r.GetByBarberIdAndDayAsync(1, 1)).ReturnsAsync(workHours);
        _mockAppointmentRepository.Setup(r => r.GetByBarberIdAsync(1)).ReturnsAsync(new List<AppointmentModel>());

        // Act
        var result = await _service.GetAvailableTimeSlotsAsync(1, date, 30);

        // Assert
        result.Should().HaveCount(4); // 9:00, 9:30, 10:00, 10:30
        result.Should().Contain(new DateTime(2025, 10, 13, 9, 0, 0, DateTimeKind.Unspecified));
        result.Should().Contain(new DateTime(2025, 10, 13, 9, 30, 0, DateTimeKind.Unspecified));
        result.Should().Contain(new DateTime(2025, 10, 13, 10, 0, 0, DateTimeKind.Unspecified));
        result.Should().Contain(new DateTime(2025, 10, 13, 10, 30, 0, DateTimeKind.Unspecified));
    }

    [Fact]
    public async Task GetAvailableTimeSlotsAsync_WhenAppointmentsExist_ShouldExcludeConflictingSlots()
    {
        // Arrange
        var date = new DateTime(2025, 10, 13); // Monday
        var workHours = new List<BarberWorkHoursModel>
        {
            new BarberWorkHoursModel
            {
                WorkHourId = 1,
                BarberId = 1,
                DayOfWeek = 1,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(11, 0),
                IsActive = true
            }
        };
        var appointments = new List<AppointmentModel>
        {
            new AppointmentModel
            {
                AppointmentId = 1,
                BarberId = 1,
                AppointmentDate = new DateTime(2025, 10, 13, 9, 30, 0),
                Status = "Confirmed"
            }
        };

        _mockWorkHoursRepository.Setup(r => r.GetByBarberIdAndDayAsync(1, 1)).ReturnsAsync(workHours);
        _mockAppointmentRepository.Setup(r => r.GetByBarberIdAsync(1)).ReturnsAsync(appointments);

        // Act
        var result = await _service.GetAvailableTimeSlotsAsync(1, date, 30);

        // Assert
        result.Should().HaveCount(3); // 9:00, 10:00, and 10:30 should be available
        result.Should().Contain(new DateTime(2025, 10, 13, 9, 0, 0, DateTimeKind.Unspecified));
        result.Should().NotContain(new DateTime(2025, 10, 13, 9, 30, 0, DateTimeKind.Unspecified)); // Booked (9:30-10:00)
        result.Should().Contain(new DateTime(2025, 10, 13, 10, 0, 0, DateTimeKind.Unspecified)); // Available (10:00-10:30 doesn't overlap)
        result.Should().Contain(new DateTime(2025, 10, 13, 10, 30, 0, DateTimeKind.Unspecified));
    }

    [Fact]
    public async Task GetAvailableTimeSlotsAsync_ShouldIgnoreCancelledAppointments()
    {
        // Arrange
        var date = new DateTime(2025, 10, 13); // Monday
        var workHours = new List<BarberWorkHoursModel>
        {
            new BarberWorkHoursModel
            {
                WorkHourId = 1,
                BarberId = 1,
                DayOfWeek = 1,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(10, 0),
                IsActive = true
            }
        };
        var appointments = new List<AppointmentModel>
        {
            new AppointmentModel
            {
                AppointmentId = 1,
                BarberId = 1,
                AppointmentDate = new DateTime(2025, 10, 13, 9, 0, 0),
                Status = "Cancelled" // Should be ignored
            }
        };

        _mockWorkHoursRepository.Setup(r => r.GetByBarberIdAndDayAsync(1, 1)).ReturnsAsync(workHours);
        _mockAppointmentRepository.Setup(r => r.GetByBarberIdAsync(1)).ReturnsAsync(appointments);

        // Act
        var result = await _service.GetAvailableTimeSlotsAsync(1, date, 30);

        // Assert
        result.Should().HaveCount(2); // Both slots available since appointment is cancelled
        result.Should().Contain(new DateTime(2025, 10, 13, 9, 0, 0, DateTimeKind.Unspecified));
        result.Should().Contain(new DateTime(2025, 10, 13, 9, 30, 0, DateTimeKind.Unspecified));
    }
}
