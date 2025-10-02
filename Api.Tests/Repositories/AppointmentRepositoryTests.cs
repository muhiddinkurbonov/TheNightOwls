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
using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Repositories;

public class AppointmentRepositoryTest : RepositoryTestBase
{
    // The repositories being tested/used for setup
    private readonly BarberRepository _barberRepository;
    private readonly CustomerRepository _customerRepository;
    private readonly ServiceRepository _serviceRepository;
    private readonly AppointmentRepository _appointmentRepository;
    private readonly BarberServiceRepository _barberServiceRepository; 

    // Sample data constants
    private const int TEST_APPOINTMENT_ID = 100;
    private const int TEST_CUSTOMER_ID = 50;
    private static readonly DateTime TEST_DATE_TODAY = DateTime.UtcNow.Date;
    private static readonly DateTime TEST_DATE_TOMORROW = DateTime.UtcNow.Date.AddDays(1);

    public AppointmentRepositoryTest()
    {
        _barberRepository = new BarberRepository(_context);
        _customerRepository = new CustomerRepository(_context);
        _serviceRepository = new ServiceRepository(_context);
        _appointmentRepository = new AppointmentRepository(_context);
        _barberServiceRepository = new BarberServiceRepository(_context);
    }

    /// Seeds core data required for appointment tests.
    private async Task SeedCoreDataAsync()
    {
        // Ensure data is cleared before seeding for independent tests
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();

        var barber = new BarberModel
        {
            BarberId = 1,
            Username = "sample_barber",
            Name = "Sample Barber",
            Specialty = "Classic Cuts",
            ContactInfo = "415-256-8844"
        };
        var customer = new CustomerModel
        {
            CustomerId = TEST_CUSTOMER_ID, // Use constant ID
            Username = "sample_customer",
            Name = "Sample Customer",
            ContactInfo = "415-256-8844"
        };
        var service = new ServiceModel
        {
            ServiceId = 1,
            ServiceName = "Haircut",
            ServicePrice = 20
        };

        // Appointment 1: AppointmentId = TEST_APPOINTMENT_ID, Date = Today, Customer = TEST_CUSTOMER_ID
        var appointment1 = new AppointmentModel
        {
            AppointmentId = TEST_APPOINTMENT_ID,
            Status = "Pending",
            BarberId = barber.BarberId,
            CustomerId = customer.CustomerId,
            ServiceId = service.ServiceId,
            appointmentDate = TEST_DATE_TODAY, 
        };

        // Appointment 2: AppointmentId = 101, Date = Tomorrow, Customer = 51 (Different customer)
        var appointment2 = new AppointmentModel
        {
            AppointmentId = 101,
            Status = "Pending", 
            BarberId = barber.BarberId,
            CustomerId = 51,
            ServiceId = service.ServiceId,
            appointmentDate = TEST_DATE_TOMORROW,
        };

        _context.barberTable.Add(barber);
        _context.customerTable.Add(customer);
        _context.customerTable.Add(new CustomerModel { CustomerId = 51, Username = "cust2", Name = "Customer 2", ContactInfo = "415-244-8844" });
        _context.serviceTable.Add(service);
        _context.appointmentTable.Add(appointment1);
        _context.appointmentTable.Add(appointment2);

        await _context.SaveChangesAsync();
    }
    

    [Fact]
    public async Task GetByIdAsync_WhenAppointmentExists_ShouldReturnAppointment()
    {
        // Arrange
        await SeedCoreDataAsync();

        // Act
        var found = await _appointmentRepository.GetByIdAsync(TEST_APPOINTMENT_ID);

        // Assert
        found.Should().NotBeNull();
        found.AppointmentId.Should().Be(TEST_APPOINTMENT_ID);
        found.CustomerId.Should().Be(TEST_CUSTOMER_ID);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAppointmentDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await SeedCoreDataAsync();
        var nonExistentId = 999;

        // Act
        var found = await _appointmentRepository.GetByIdAsync(nonExistentId);

        // Assert
        found.Should().BeNull();
    }
    

    [Fact]
    public async Task GetAll_ShouldReturnAllAppointments()
    {
        // Arrange
        await SeedCoreDataAsync();

        // Act
        var appointments = await _appointmentRepository.GetAll();

        // Assert
        appointments.Should().NotBeEmpty();
        appointments.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_WhenNoAppointmentsExist_ShouldReturnEmptyList()
    {
        // Arrange
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();

        // Act
        var appointments = await _appointmentRepository.GetAll();

        // Assert
        appointments.Should().BeEmpty();
    }


    [Fact]
    public async Task GetApptsByDate_ShouldReturnAppointmentsOnTargetDate()
    {
        // Arrange
        await SeedCoreDataAsync();

        // Act
        var appointments = await _appointmentRepository.GetApptsByDate(TEST_DATE_TODAY.AddHours(15)); // Pass a DateTime with a time component

        // Assert
        appointments.Should().HaveCount(1);
        appointments.First().AppointmentId.Should().Be(TEST_APPOINTMENT_ID);
        // Assert that the date part matches
        appointments.All(a => a.appointmentDate.Date == TEST_DATE_TODAY).Should().BeTrue();
    }

    [Fact]
    public async Task GetApptsByDate_WhenNoAppointmentsOnDate_ShouldReturnEmptyList()
    {
        // Arrange
        await SeedCoreDataAsync();
        var futureDate = DateTime.UtcNow.Date.AddDays(5);

        // Act
        var appointments = await _appointmentRepository.GetApptsByDate(futureDate);

        // Assert
        appointments.Should().BeEmpty();
    }


    [Fact]
    public async Task GetByCustomerId_ShouldReturnAppointmentsForCustomer()
    {
        // Arrange
        await SeedCoreDataAsync();

        // Act
        var appointments = await _appointmentRepository.GetByCustomerId(TEST_CUSTOMER_ID);

        // Assert
        appointments.Should().HaveCount(1);
        appointments.First().AppointmentId.Should().Be(TEST_APPOINTMENT_ID);
        appointments.All(a => a.CustomerId == TEST_CUSTOMER_ID).Should().BeTrue();
    }

    [Fact]
    public async Task GetByCustomerId_WhenCustomerHasNoAppointments_ShouldReturnEmptyList()
    {
        // Arrange
        await SeedCoreDataAsync();
        var nonAppointmentCustomer = 999;

        // Act
        var appointments = await _appointmentRepository.GetByCustomerId(nonAppointmentCustomer);

        // Assert
        appointments.Should().BeEmpty();
    }


    [Fact]
    public async Task AddAppointment_ShouldAddNewAppointment()
    {
        // Arrange
        await SeedCoreDataAsync();
        var newApptId = 200;
        var newAppointment = new AppointmentModel
        {
            AppointmentId = newApptId,
            Status = "Pending",
            BarberId = 1,
            CustomerId = 51,
            ServiceId = 1,
            appointmentDate = DateTime.UtcNow.AddDays(2),
        };

        // Act
        var addedAppointment = await _appointmentRepository.AddAppointment(newAppointment);
        await _context.SaveChangesAsync();
        var foundInDb = await _appointmentRepository.GetByIdAsync(newApptId);

        // Assert
        addedAppointment.Should().NotBeNull();
        addedAppointment.AppointmentId.Should().Be(newApptId);
        foundInDb.Should().NotBeNull();
        (await _appointmentRepository.GetAll()).Should().HaveCount(3);
    }

    [Fact]
    public async Task AddAppointment_WhenAppointmentAlreadyExists_ShouldReturnExistingAppointment()
    {
        // Arrange
        await SeedCoreDataAsync();
        var existingAppointment = new AppointmentModel
        {
            AppointmentId = TEST_APPOINTMENT_ID,
            Status = "Pending",
            BarberId = 1,
            CustomerId = TEST_CUSTOMER_ID,
            ServiceId = 1,
            appointmentDate = TEST_DATE_TODAY,
        };
        
        // Act
        var result = await _appointmentRepository.AddAppointment(existingAppointment);
        
        // Assert
        result.Should().NotBeNull();
        result.AppointmentId.Should().Be(TEST_APPOINTMENT_ID);
        (await _appointmentRepository.GetAll()).Should().HaveCount(2); 
    }


    [Fact]
    public async Task UpdateAppointment_ShouldUpdateExistingAppointment()
    {
        // Arrange
        await SeedCoreDataAsync();
        var newUsername = "Updated_Customer_User";
        var updatedModel = new AppointmentModel
        {
            AppointmentId = TEST_APPOINTMENT_ID,
            Status = "Pending",
            BarberId = 1,
            CustomerId = TEST_CUSTOMER_ID,
            ServiceId = 1,
            appointmentDate = TEST_DATE_TODAY,
        };

        // Act
        var result = await _appointmentRepository.UpdateAppointment(updatedModel);
        await _context.SaveChangesAsync();
        var foundInDb = await _appointmentRepository.GetByIdAsync(TEST_APPOINTMENT_ID);

        // Assert
        result.Should().NotBeNull();
        foundInDb.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAppointment_WhenAppointmentDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await SeedCoreDataAsync();
        var nonExistentModel = new AppointmentModel
        {
            AppointmentId = 999,
            Status = "Pending",
            BarberId = 1,
            CustomerId = 1,
            ServiceId = 1,
            appointmentDate = DateTime.UtcNow,
        };

        // Act
        var result = await _appointmentRepository.UpdateAppointment(nonExistentModel);

        // Assert
        result.Should().BeNull();
    }
    

    [Fact]
    public async Task DeleteApptById_ShouldDeleteAppointment()
    {
        // Arrange
        await SeedCoreDataAsync();
        (await _appointmentRepository.GetAll()).Should().HaveCount(2);

        // Act
        var deletedAppt = await _appointmentRepository.DeleteApptById(TEST_APPOINTMENT_ID);
        await _context.SaveChangesAsync();

        // Assert
        deletedAppt.Should().NotBeNull();
        deletedAppt.AppointmentId.Should().Be(TEST_APPOINTMENT_ID);
        (await _appointmentRepository.GetByIdAsync(TEST_APPOINTMENT_ID)).Should().BeNull();
        (await _appointmentRepository.GetAll()).Should().HaveCount(1);
    }

    [Fact]
    public async Task DeleteApptById_WhenAppointmentDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await SeedCoreDataAsync();
        var nonExistentId = 999;

        // Act
        var deletedAppt = await _appointmentRepository.DeleteApptById(nonExistentId);
        await _context.SaveChangesAsync();

        // Assert
        deletedAppt.Should().BeNull();
        (await _appointmentRepository.GetAll()).Should().HaveCount(2); // Count should be unchanged
    }
}