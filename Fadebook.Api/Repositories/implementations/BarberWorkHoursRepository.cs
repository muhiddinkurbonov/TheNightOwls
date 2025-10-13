using Microsoft.EntityFrameworkCore;
using Fadebook.DB;
using Fadebook.Models;

namespace Fadebook.Repositories;

public class BarberWorkHoursRepository(FadebookDbContext _dbContext) : IBarberWorkHoursRepository
{
    public async Task<IEnumerable<BarberWorkHoursModel>> GetAllAsync()
    {
        return await _dbContext.barberWorkHoursTable
            .Include(bwh => bwh.Barber)
            .ToListAsync();
    }

    public async Task<BarberWorkHoursModel?> GetByIdAsync(int workHourId)
    {
        return await _dbContext.barberWorkHoursTable
            .Include(bwh => bwh.Barber)
            .FirstOrDefaultAsync(bwh => bwh.WorkHourId == workHourId);
    }

    public async Task<IEnumerable<BarberWorkHoursModel>> GetByBarberIdAsync(int barberId)
    {
        return await _dbContext.barberWorkHoursTable
            .Include(bwh => bwh.Barber)
            .Where(bwh => bwh.BarberId == barberId)
            .OrderBy(bwh => bwh.DayOfWeek)
            .ThenBy(bwh => bwh.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<BarberWorkHoursModel>> GetByBarberIdAndDayAsync(int barberId, int dayOfWeek)
    {
        return await _dbContext.barberWorkHoursTable
            .Include(bwh => bwh.Barber)
            .Where(bwh => bwh.BarberId == barberId && bwh.DayOfWeek == dayOfWeek && bwh.IsActive)
            .OrderBy(bwh => bwh.StartTime)
            .ToListAsync();
    }

    public async Task<BarberWorkHoursModel> AddAsync(BarberWorkHoursModel workHours)
    {
        var entry = await _dbContext.barberWorkHoursTable.AddAsync(workHours);
        return entry.Entity;
    }

    public async Task<BarberWorkHoursModel?> UpdateAsync(int workHourId, BarberWorkHoursModel workHours)
    {
        var existing = await _dbContext.barberWorkHoursTable.FindAsync(workHourId);
        if (existing is null) return null;

        existing.DayOfWeek = workHours.DayOfWeek;
        existing.StartTime = workHours.StartTime;
        existing.EndTime = workHours.EndTime;
        existing.IsActive = workHours.IsActive;

        _dbContext.barberWorkHoursTable.Update(existing);
        return existing;
    }

    public async Task<BarberWorkHoursModel?> RemoveByIdAsync(int workHourId)
    {
        var workHours = await _dbContext.barberWorkHoursTable.FindAsync(workHourId);
        if (workHours is null) return null;

        _dbContext.barberWorkHoursTable.Remove(workHours);
        return workHours;
    }

    public async Task<bool> IsBarberAvailableAsync(int barberId, DateTime appointmentDateTime, int durationMinutes = 30)
    {
        // Get day of week (0 = Sunday, 6 = Saturday)
        int dayOfWeek = (int)appointmentDateTime.DayOfWeek;
        TimeOnly appointmentTime = TimeOnly.FromDateTime(appointmentDateTime);
        TimeOnly appointmentEndTime = appointmentTime.AddMinutes(durationMinutes);

        // Check if there's any active work hour slot that covers this appointment time
        var workHours = await _dbContext.barberWorkHoursTable
            .Where(bwh => bwh.BarberId == barberId
                && bwh.DayOfWeek == dayOfWeek
                && bwh.IsActive
                && bwh.StartTime <= appointmentTime
                && bwh.EndTime >= appointmentEndTime)
            .AnyAsync();

        return workHours;
    }
}
