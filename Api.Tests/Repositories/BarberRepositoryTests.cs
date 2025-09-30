using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Fadebook.DB;
using Fadebook.Models;
using Fadebook.Repositories;
using FluentAssertions;

namespace Api.Tests.Repositories;

public class BarberRepositoryTests
{
    private static NightOwlsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<NightOwlsDbContext>()
            .UseInMemoryDatabase(databaseName: $"NightOwlsTestDb_{Guid.NewGuid()}")
            .Options;
        return new NightOwlsDbContext(options);
    }

    [Fact]
    public async Task GetByIdAsync_WhenBarberExists_ShouldReturnBarber()
    {
        // Arrange
        await using var db = CreateDbContext();
        var repo = new BarberRepository(db);
        var barber = new BarberModel
        {
            Username = "john_doe",
            Name = "John Doe",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        var added = await repo.AddAsync(barber);
        await db.SaveChangesAsync();

        // Act
        // Repository does not call SaveChanges; test calls it to persist
        var found = await repo.GetByIdAsync(new BarberModel { BarberId = added.BarberId });

        // Assert
        found.Should().NotBeNull();
        found.BarberId.Should().Be(added.BarberId);
        found.Username.Should().Be("john_doe");
    }

     [Fact]
    public async Task GetByIdAsync_WhenBarberDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await using var db = CreateDbContext();
        var repo = new BarberRepository(db);
        var barber = new BarberModel
        {
            Username = "john_doe",
            Name = "John Doe",
            Specialty = "Fades",
            ContactInfo = "john@example.com"
        };
        var added = await repo.AddAsync(barber);
        await db.SaveChangesAsync();

        // Act
        // Repository does not call SaveChanges; test calls it to persist
        var found = await repo.GetByIdAsync(new BarberModel { BarberId = added.BarberId + 1 });

        // Assert
        found.Should().BeNull();
    }
}
