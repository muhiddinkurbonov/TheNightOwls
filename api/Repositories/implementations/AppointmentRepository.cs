
using Fadebook.Models;
using Fadebook.DB;
using Microsoft.EntityFrameworkCore;

namespace Fadebook.Repositories;

public class AppointmentRepository(
    FadebookDbContext _fadebookDbContext
    ) : IAppointmentRepository
{
    public async Task<AppointmentModel?> GetByIdAsync(int appointmentId)
    {
        return await _fadebookDbContext.appointmentTable
            .Include(a => a.Customer)
            .Include(a => a.Barber)
            .Include(a => a.Service)
            .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
    }

    public async Task<IEnumerable<AppointmentModel>> GetAllAsync()
    {
        return await _fadebookDbContext.appointmentTable
            .Include(a => a.Customer)
            .Include(a => a.Barber)
            .Include(a => a.Service)
            .ToListAsync();
    }

    public async Task<IEnumerable<AppointmentModel>> GetByDateAsync(DateTime targetDate)
    {
        // Ensure we're comparing dates in UTC
        var startOfDay = DateTime.SpecifyKind(targetDate.Date, DateTimeKind.Utc);
        var endOfDay = startOfDay.AddDays(1);

        return await _fadebookDbContext.appointmentTable
            .Include(a => a.Customer)
            .Include(a => a.Barber)
            .Include(a => a.Service)
            .Where(a => a.AppointmentDate >= startOfDay && a.AppointmentDate < endOfDay)
            .ToListAsync();
    }

    public async Task<IEnumerable<AppointmentModel>> GetByCustomerIdAsync(int customerId)
    {
        return await _fadebookDbContext.appointmentTable
            .Include(a => a.Customer)
            .Include(a => a.Barber)
            .Include(a => a.Service)
            .Where(a => a.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<IEnumerable<AppointmentModel>> GetByBarberIdAsync(int barberId)
    {
        return await _fadebookDbContext.appointmentTable
            .Include(a => a.Customer)
            .Include(a => a.Barber)
            .Include(a => a.Service)
            .Where(a => a.BarberId == barberId)
            .ToListAsync();
    }

    public async Task<IEnumerable<AppointmentModel>> GetByServiceIdAsync(int serviceId)
    {
        return await _fadebookDbContext.appointmentTable
            .Include(a => a.Customer)
            .Include(a => a.Barber)
            .Include(a => a.Service)
            .Where(a => a.ServiceId == serviceId)
            .ToListAsync();
    }

    public async Task<AppointmentModel> AddAsync(AppointmentModel appointmentModel)
    {
        // Only check for existing appointment if ID is not 0 (new appointments should have ID 0)
        if (appointmentModel.AppointmentId != 0)
        {
            var foundAppointment = await this.GetByIdAsync(appointmentModel.AppointmentId);
            if (foundAppointment != null) return foundAppointment;
        }

        if (!await ValidateForeignKeysAsync(appointmentModel))
            return null;

        // Reset ID to 0 to let database auto-generate it
        appointmentModel.AppointmentId = 0;
        await _fadebookDbContext.appointmentTable.AddAsync(appointmentModel);
        return appointmentModel;
    }

    public async Task<AppointmentModel> UpdateAsync(int appointmentId, AppointmentModel appointmentModel)
    {
        var foundAppointmentModel = await this.GetByIdAsync(appointmentId);
        if (foundAppointmentModel is null) return null;

        if (!await ValidateForeignKeysAsync(appointmentModel))
            return null;

        foundAppointmentModel.BarberId = appointmentModel.BarberId;
        foundAppointmentModel.CustomerId = appointmentModel.CustomerId;
        foundAppointmentModel.ServiceId = appointmentModel.ServiceId;
        foundAppointmentModel.AppointmentDate = appointmentModel.AppointmentDate;
        foundAppointmentModel.Status = appointmentModel.Status;
        _fadebookDbContext.appointmentTable.Update(foundAppointmentModel);
        return foundAppointmentModel;
    }

    public async Task<AppointmentModel> RemoveByIdAsync(int appointmentId)
    {
        var foundAppointmentModel = await this.GetByIdAsync(appointmentId);
        if (foundAppointmentModel is null) return null;
        _fadebookDbContext.appointmentTable.Remove(foundAppointmentModel);
        return foundAppointmentModel;
    }

    private async Task<bool> ValidateForeignKeysAsync(AppointmentModel appointmentModel)
    {
        var customerExists = await _fadebookDbContext.customerTable
            .AnyAsync(c => c.CustomerId == appointmentModel.CustomerId);
        if (!customerExists) return false;

        var barberExists = await _fadebookDbContext.barberTable
            .AnyAsync(b => b.BarberId == appointmentModel.BarberId);
        if (!barberExists) return false;

        var serviceExists = await _fadebookDbContext.serviceTable
            .AnyAsync(s => s.ServiceId == appointmentModel.ServiceId);
        if (!serviceExists) return false;

        return true;
    }
}

