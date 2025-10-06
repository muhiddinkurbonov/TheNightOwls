
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
        return await _fadebookDbContext.appointmentTable.FindAsync(appointmentId);
    }
    public async Task<IEnumerable<AppointmentModel>> GetAllAsync()
    {
        return await _fadebookDbContext.appointmentTable.ToListAsync();
    }
    public async Task<IEnumerable<AppointmentModel>> GetByDateAsync(DateTime targetDate)
    {
        return await _fadebookDbContext.appointmentTable
            .Where(a => a.AppointmentDate.Date == targetDate.Date)
            .ToListAsync();
    }
    public async Task<IEnumerable<AppointmentModel>> GetByCustomerIdAsync(int customerId)
    {
        return await _fadebookDbContext.appointmentTable
            .Where(a => a.CustomerId == customerId)
            .ToListAsync();
    }
    public async Task<IEnumerable<AppointmentModel>> GetByBarberIdAsync(int barberId)
    {
        return await _fadebookDbContext.appointmentTable
            .Where(a => a.BarberId == barberId)
            .ToListAsync();
    }
    public async Task<IEnumerable<AppointmentModel>> GetByServiceIdAsync(int serviceId)
    {
        return await _fadebookDbContext.appointmentTable
            .Where(a => a.ServiceId == serviceId)
            .ToListAsync();
    }
    public async Task<AppointmentModel> AddAsync(AppointmentModel appointmentModel)
    {
        await _fadebookDbContext.appointmentTable.AddAsync(appointmentModel);
        return appointmentModel;
    }

    public async Task<AppointmentModel> UpdateAsync(int appointmentId, AppointmentModel appointmentModel)
    {
        var foundAppointmentModel = await this.GetByIdAsync(appointmentId);
        if (foundAppointmentModel is null)
            throw new KeyNotFoundException($"Appointment with ID {appointmentId} was not found.");
        foundAppointmentModel.Update(appointmentModel);
        _fadebookDbContext.appointmentTable.Update(foundAppointmentModel);
        return appointmentModel;
    }
    public async Task<AppointmentModel> RemoveByIdAsync(int appointmentId)
    {
        var foundAppointmentModel = await this.GetByIdAsync(appointmentId);
        if (foundAppointmentModel is null)
            throw new KeyNotFoundException($"Appointment with ID {appointmentId} was not found.");
        _fadebookDbContext.appointmentTable.Remove(foundAppointmentModel);
        return foundAppointmentModel;
    }
}

