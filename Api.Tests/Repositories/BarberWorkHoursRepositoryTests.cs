using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Fadebook.Models;
using Fadebook.Repositories;
using FluentAssertions;
using Fadebook.Api.Tests.TestUtilities;

namespace Api.Tests.Repositories;

public class BarberWorkHoursRepositoryTests : RepositoryTestBase
{
    private readonly BarberWorkHoursRepository _repo;

    public BarberWorkHoursRepositoryTests()
    {
        _repo = new BarberWorkHoursRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_WhenWorkHoursExist_ShouldReturnAllWorkHours()
    {
        // Arrange
        var barber = new BarberModel
        {
            BarberId = 1,
            Username = "john_barber",
            Name = "John Barber",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);

        var workHours1 = new BarberWorkHoursModel
        {
            WorkHourId = 1,
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };
        var workHours2 = new BarberWorkHoursModel
        {
            WorkHourId = 2,
            BarberId = 1,
            DayOfWeek = 2,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(18, 0),
            IsActive = true
        };
        _context.barberWorkHoursTable.Add(workHours1);
        _context.barberWorkHoursTable.Add(workHours2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(w => w.WorkHourId == 1);
        result.Should().Contain(w => w.WorkHourId == 2);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoWorkHours_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repo.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WhenWorkHoursExist_ShouldReturnWorkHours()
    {
        // Arrange
        var barber = new BarberModel
        {
            BarberId = 1,
            Username = "john_barber",
            Name = "John Barber",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);

        var workHours = new BarberWorkHoursModel
        {
            WorkHourId = 1,
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };
        _context.barberWorkHoursTable.Add(workHours);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.WorkHourId.Should().Be(1);
        result.BarberId.Should().Be(1);
        result.DayOfWeek.Should().Be(1);
        result.StartTime.Should().Be(new TimeOnly(9, 0));
        result.EndTime.Should().Be(new TimeOnly(17, 0));
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_WhenWorkHoursDoNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _repo.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByBarberIdAsync_WhenWorkHoursExist_ShouldReturnWorkHoursSortedByDayAndTime()
    {
        // Arrange
        var barber = new BarberModel
        {
            BarberId = 1,
            Username = "john_barber",
            Name = "John Barber",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);

        var workHours1 = new BarberWorkHoursModel
        {
            WorkHourId = 1,
            BarberId = 1,
            DayOfWeek = 2, // Tuesday
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };
        var workHours2 = new BarberWorkHoursModel
        {
            WorkHourId = 2,
            BarberId = 1,
            DayOfWeek = 1, // Monday
            StartTime = new TimeOnly(14, 0),
            EndTime = new TimeOnly(18, 0),
            IsActive = true
        };
        var workHours3 = new BarberWorkHoursModel
        {
            WorkHourId = 3,
            BarberId = 1,
            DayOfWeek = 1, // Monday
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(13, 0),
            IsActive = true
        };
        _context.barberWorkHoursTable.Add(workHours1);
        _context.barberWorkHoursTable.Add(workHours2);
        _context.barberWorkHoursTable.Add(workHours3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.GetByBarberIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        var resultList = result.ToList();
        // Should be sorted by DayOfWeek then StartTime
        resultList[0].DayOfWeek.Should().Be(1); // Monday morning
        resultList[0].StartTime.Should().Be(new TimeOnly(9, 0));
        resultList[1].DayOfWeek.Should().Be(1); // Monday afternoon
        resultList[1].StartTime.Should().Be(new TimeOnly(14, 0));
        resultList[2].DayOfWeek.Should().Be(2); // Tuesday
    }

    [Fact]
    public async Task GetByBarberIdAsync_WhenNoWorkHours_ShouldReturnEmptyList()
    {
        // Arrange
        var barber = new BarberModel
        {
            BarberId = 1,
            Username = "john_barber",
            Name = "John Barber",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.GetByBarberIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByBarberIdAndDayAsync_WhenWorkHoursExist_ShouldReturnOnlyActiveWorkHoursForDay()
    {
        // Arrange
        var barber = new BarberModel
        {
            BarberId = 1,
            Username = "john_barber",
            Name = "John Barber",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);

        var workHours1 = new BarberWorkHoursModel
        {
            WorkHourId = 1,
            BarberId = 1,
            DayOfWeek = 1, // Monday
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };
        var workHours2 = new BarberWorkHoursModel
        {
            WorkHourId = 2,
            BarberId = 1,
            DayOfWeek = 1, // Monday but inactive
            StartTime = new TimeOnly(18, 0),
            EndTime = new TimeOnly(20, 0),
            IsActive = false
        };
        var workHours3 = new BarberWorkHoursModel
        {
            WorkHourId = 3,
            BarberId = 1,
            DayOfWeek = 2, // Tuesday
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };
        _context.barberWorkHoursTable.Add(workHours1);
        _context.barberWorkHoursTable.Add(workHours2);
        _context.barberWorkHoursTable.Add(workHours3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.GetByBarberIdAndDayAsync(1, 1); // Monday

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().WorkHourId.Should().Be(1);
        result.First().IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task AddAsync_ShouldAddWorkHours()
    {
        // Arrange
        var barber = new BarberModel
        {
            BarberId = 1,
            Username = "john_barber",
            Name = "John Barber",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);
        await _context.SaveChangesAsync();

        var workHours = new BarberWorkHoursModel
        {
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };

        // Act
        var result = await _repo.AddAsync(workHours);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.BarberId.Should().Be(1);
        result.DayOfWeek.Should().Be(1);

        var saved = await _context.barberWorkHoursTable.FindAsync(result.WorkHourId);
        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WhenWorkHoursExist_ShouldUpdateWorkHours()
    {
        // Arrange
        var barber = new BarberModel
        {
            BarberId = 1,
            Username = "john_barber",
            Name = "John Barber",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);

        var workHours = new BarberWorkHoursModel
        {
            WorkHourId = 1,
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };
        _context.barberWorkHoursTable.Add(workHours);
        await _context.SaveChangesAsync();

        var updatedWorkHours = new BarberWorkHoursModel
        {
            BarberId = 1,
            DayOfWeek = 2,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(18, 0),
            IsActive = false
        };

        // Act
        var result = await _repo.UpdateAsync(1, updatedWorkHours);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.WorkHourId.Should().Be(1);
        result.DayOfWeek.Should().Be(2);
        result.StartTime.Should().Be(new TimeOnly(10, 0));
        result.EndTime.Should().Be(new TimeOnly(18, 0));
        result.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_WhenWorkHoursDoNotExist_ShouldReturnNull()
    {
        // Arrange
        var updatedWorkHours = new BarberWorkHoursModel
        {
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };

        // Act
        var result = await _repo.UpdateAsync(999, updatedWorkHours);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RemoveByIdAsync_WhenWorkHoursExist_ShouldRemoveWorkHours()
    {
        // Arrange
        var barber = new BarberModel
        {
            BarberId = 1,
            Username = "john_barber",
            Name = "John Barber",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);

        var workHours = new BarberWorkHoursModel
        {
            WorkHourId = 1,
            BarberId = 1,
            DayOfWeek = 1,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };
        _context.barberWorkHoursTable.Add(workHours);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.RemoveByIdAsync(1);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.WorkHourId.Should().Be(1);

        var removed = await _context.barberWorkHoursTable.FindAsync(1);
        removed.Should().BeNull();
    }

    [Fact]
    public async Task RemoveByIdAsync_WhenWorkHoursDoNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _repo.RemoveByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task IsBarberAvailableAsync_WhenBarberIsAvailable_ShouldReturnTrue()
    {
        // Arrange
        var barber = new BarberModel
        {
            BarberId = 1,
            Username = "john_barber",
            Name = "John Barber",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);

        var workHours = new BarberWorkHoursModel
        {
            WorkHourId = 1,
            BarberId = 1,
            DayOfWeek = 1, // Monday
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };
        _context.barberWorkHoursTable.Add(workHours);
        await _context.SaveChangesAsync();

        // Create a Monday at 10:00 AM
        var appointmentDateTime = new DateTime(2025, 10, 13, 10, 0, 0); // Monday

        // Act
        var result = await _repo.IsBarberAvailableAsync(1, appointmentDateTime, 30);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsBarberAvailableAsync_WhenAppointmentExceedsWorkHours_ShouldReturnFalse()
    {
        // Arrange
        var barber = new BarberModel
        {
            BarberId = 1,
            Username = "john_barber",
            Name = "John Barber",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);

        var workHours = new BarberWorkHoursModel
        {
            WorkHourId = 1,
            BarberId = 1,
            DayOfWeek = 1, // Monday
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };
        _context.barberWorkHoursTable.Add(workHours);
        await _context.SaveChangesAsync();

        // Create a Monday at 16:45 (appointment would end at 17:15, past work hours)
        var appointmentDateTime = new DateTime(2025, 10, 13, 16, 45, 0);

        // Act
        var result = await _repo.IsBarberAvailableAsync(1, appointmentDateTime, 30);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsBarberAvailableAsync_WhenWorkHoursAreInactive_ShouldReturnFalse()
    {
        // Arrange
        var barber = new BarberModel
        {
            BarberId = 1,
            Username = "john_barber",
            Name = "John Barber",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);

        var workHours = new BarberWorkHoursModel
        {
            WorkHourId = 1,
            BarberId = 1,
            DayOfWeek = 1, // Monday
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = false // Inactive
        };
        _context.barberWorkHoursTable.Add(workHours);
        await _context.SaveChangesAsync();

        var appointmentDateTime = new DateTime(2025, 10, 13, 10, 0, 0);

        // Act
        var result = await _repo.IsBarberAvailableAsync(1, appointmentDateTime, 30);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsBarberAvailableAsync_WhenNoWorkHoursForDay_ShouldReturnFalse()
    {
        // Arrange
        var barber = new BarberModel
        {
            BarberId = 1,
            Username = "john_barber",
            Name = "John Barber",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);

        var workHours = new BarberWorkHoursModel
        {
            WorkHourId = 1,
            BarberId = 1,
            DayOfWeek = 2, // Tuesday
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };
        _context.barberWorkHoursTable.Add(workHours);
        await _context.SaveChangesAsync();

        // Try Monday when only Tuesday is available
        var appointmentDateTime = new DateTime(2025, 10, 13, 10, 0, 0); // Monday

        // Act
        var result = await _repo.IsBarberAvailableAsync(1, appointmentDateTime, 30);

        // Assert
        result.Should().BeFalse();
    }
}
