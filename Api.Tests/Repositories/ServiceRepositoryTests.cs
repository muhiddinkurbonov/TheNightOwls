using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Fadebook.DB;
using Fadebook.Models;
using Fadebook.Repositories;

namespace Api.Tests.Repositories;

public class ServiceRepositoryTests
{
    private static NightOwlsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<NightOwlsDbContext>()
            .UseInMemoryDatabase(databaseName: $"NightOwlsTestDb_Service_{Guid.NewGuid()}")
            .Options;
        return new NightOwlsDbContext(options);
    }

    // public async Task GetByIdAsync_WhenServiceExists_ShouldReturnService()
    // {
    //     // Arrange
    //     await using var db = CreateDbContext();
    //     var repo = new ServiceRepository(db);
    //     var service = new ServiceModel
    //     {
    //         ServiceName = "john_doe",
    //         ServicePrice = 10
    //     };
    //     var added = await repo.AddAsync(service);
    //     await db.SaveChangesAsync();

    //     // Act
    //     // Repository does not call SaveChanges; test calls it to persist
    //     var found = await repo.GetByIdAsync(new ServiceModel { ServiceId = added.ServiceId });

    //     // Assert
    //     found.Should().NotBeNull();
    //     found.ServiceId.Should().Be(added.ServiceId);
    //     found.ServiceName.Should().Be("john_doe");
    // }

    //  [Fact]
    // public async Task GetByIdAsync_WhenServiceDoesNotExist_ShouldReturnNull()
    // {
    //     // Arrange
    //     await using var db = CreateDbContext();
    //     var repo = new ServiceRepository(db);
    //     var service = new ServiceModel
    //     {
    //         ServiceName = "john_doe",
    //         ServicePrice = 10
    //     };
    //     var added = await repo.AddAsync(service);
    //     await db.SaveChangesAsync();

    //     // Act
    //     // Repository does not call SaveChanges; test calls it to persist
    //     var found = await repo.GetByIdAsync(new ServiceModel { ServiceId = added.ServiceId + 1 });

    //     // Assert
    //     found.Should().BeNull();
    // }
}
