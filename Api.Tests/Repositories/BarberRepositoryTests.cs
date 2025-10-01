using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using Fadebook.DB;
using Fadebook.Models;
using Fadebook.Repositories;
using FluentAssertions;
using Fadebook.Api.Tests.TestUtilities;

namespace Api.Tests.Repositories;

public class BarberRepositoryTests: RepositoryTestBase
{
   private readonly BarberRepository _repo;

    public BarberRepositoryTests()
    {
        _repo = new BarberRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WhenBarberExists_ShouldReturnBarber()
    {
        // Arrange
        var barber = new BarberModel
        {
            Username = "john_doe",
            Name = "John Doe",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);
        await _context.SaveChangesAsync();

        // Act
        // Repository does not call SaveChanges; test calls it to persist
        var found = await _repo.GetByIdAsync(new BarberModel { BarberId = barber.BarberId });

        // Assert
        found.Should().NotBeNull();
        found.BarberId.Should().Be(barber.BarberId);
        found.Username.Should().Be("john_doe");
    }

     [Fact]
    public async Task GetByIdAsync_WhenBarberDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var barber = new BarberModel
        {
            Username = "john_doe",
            Name = "John Doe",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        _context.barberTable.Add(barber);
        await _context.SaveChangesAsync();

        // Act
        // Repository does not call SaveChanges; test calls it to persist
        var found = await _repo.GetByIdAsync(new BarberModel { BarberId = barber.BarberId + 1 });

        // Assert
        found.Should().BeNull();
    }
}
