
using Fadebook.Models;
using Fadebook.DB;
using Microsoft.EntityFrameworkCore;

namespace Fadebook.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly NightOwlsDbContext _nightOwlsDbContext;

    public AppointmentRepository(NightOwlsDbContext nightOwlsDbContext)
    {
        _nightOwlsDbContext = nightOwlsDbContext;
    }
    public async Task<AppointmentModel?> GetByIdAsync(int appointmentId)
    {
        return await _nightOwlsDbContext.appointmentTable.FindAsync(appointmentId);
    }
    public async Task<IEnumerable<AppointmentModel>> GetAll()
    {
        return await _nightOwlsDbContext.appointmentTable.ToListAsync();
    }
    public async Task<IEnumerable<AppointmentModel>> GetApptsByDate(DateTime targetDate)
    {
        return await _nightOwlsDbContext.appointmentTable
            .Where(a => a.appointmentDate.Date == targetDate.Date)
            .ToListAsync();
    }
    public async Task<IEnumerable<AppointmentModel>> GetByCustomerId(int customerId)
    {
        return await _nightOwlsDbContext.appointmentTable
            .Where(a => a.CustomerId == customerId)
            .ToListAsync();
    }
    public async Task<AppointmentModel?> AddAppointment(AppointmentModel appointmentModel)
    {
        var foundAppointment = await this.GetByIdAsync(appointmentModel.AppointmentId);
        // TODO: Throw exception to be handled -> throw, catch and return 40# code saying resource exists
        if (foundAppointment != null) return foundAppointment;
        // Validate foreign keys exist
        var customerExists = await _nightOwlsDbContext.customerTable
            .AnyAsync(c => c.CustomerId == appointmentModel.CustomerId);
        if (!customerExists) return null;

        var barberExists = await _nightOwlsDbContext.barberTable
            .AnyAsync(b => b.BarberId == appointmentModel.BarberId);
        if (!barberExists) return null;

        var serviceExists = await _nightOwlsDbContext.serviceTable
            .AnyAsync(s => s.ServiceId == appointmentModel.ServiceId);
        if (!serviceExists) return null;

        await _nightOwlsDbContext.appointmentTable.AddAsync(appointmentModel);
        await _nightOwlsDbContext.SaveChangesAsync();
        return appointmentModel;
    }

    public async Task<AppointmentModel?> UpdateAppointment(AppointmentModel appointmentModel)
    {
        var foundAppointmentModel = await this.GetByIdAsync(appointmentModel.AppointmentId);
        if (foundAppointmentModel is null) return null;

        // Validate foreign keys exist before updating
        var customerExists = await _nightOwlsDbContext.customerTable
            .AnyAsync(c => c.CustomerId == appointmentModel.CustomerId);
        if (!customerExists) return null;

        var barberExists = await _nightOwlsDbContext.barberTable
            .AnyAsync(b => b.BarberId == appointmentModel.BarberId);
        if (!barberExists) return null;

        var serviceExists = await _nightOwlsDbContext.serviceTable
            .AnyAsync(s => s.ServiceId == appointmentModel.ServiceId);
        if (!serviceExists) return null;

        foundAppointmentModel.BarberId = appointmentModel.BarberId;
        foundAppointmentModel.CustomerId = appointmentModel.CustomerId;
        foundAppointmentModel.ServiceId = appointmentModel.ServiceId;
        foundAppointmentModel.appointmentDate = appointmentModel.appointmentDate;
        foundAppointmentModel.Status = appointmentModel.Status;
        _nightOwlsDbContext.appointmentTable.Update(foundAppointmentModel);
        await _nightOwlsDbContext.SaveChangesAsync();
        return foundAppointmentModel;
    }
    public async Task<AppointmentModel?> DeleteApptById(int appointmentId)
    {
        // TODO: Throw exception for not found
        var appointment = await GetByIdAsync(appointmentId);
        if (appointment == null) return null;
        // throw new NotFoundException($"There is no barber service with id: {barberServiceId}");
        _nightOwlsDbContext.appointmentTable.Remove(appointment);
        await _nightOwlsDbContext.SaveChangesAsync();
        return appointment;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _nightOwlsDbContext.SaveChangesAsync();
    }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _nightOwlsDbContext.SaveChangesAsync(cancellationToken);
    }

}

