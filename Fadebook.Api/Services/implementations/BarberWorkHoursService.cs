using Fadebook.Models;
using Fadebook.Repositories;
using Fadebook.Exceptions;

namespace Fadebook.Services;

public class BarberWorkHoursService(
    IDbTransactionContext _dbTransactionContext,
    IBarberWorkHoursRepository _workHoursRepository,
    IAppointmentRepository _appointmentRepository
    ) : IBarberWorkHoursService
{
    public async Task<IEnumerable<BarberWorkHoursModel>> GetAllWorkHoursAsync()
    {
        return await _workHoursRepository.GetAllAsync();
    }

    public async Task<BarberWorkHoursModel> GetWorkHoursByIdAsync(int workHourId)
    {
        var workHours = await _workHoursRepository.GetByIdAsync(workHourId);
        if (workHours is null)
            throw new NotFoundException($"Work hours with ID {workHourId} not found.");
        return workHours;
    }

    public async Task<IEnumerable<BarberWorkHoursModel>> GetWorkHoursByBarberIdAsync(int barberId)
    {
        return await _workHoursRepository.GetByBarberIdAsync(barberId);
    }

    public async Task<IEnumerable<BarberWorkHoursModel>> GetWorkHoursByBarberIdAndDayAsync(int barberId, int dayOfWeek)
    {
        if (dayOfWeek < 0 || dayOfWeek > 6)
            throw new ArgumentException("DayOfWeek must be between 0 (Sunday) and 6 (Saturday).");

        return await _workHoursRepository.GetByBarberIdAndDayAsync(barberId, dayOfWeek);
    }

    public async Task<BarberWorkHoursModel> AddWorkHoursAsync(BarberWorkHoursModel workHours)
    {
        try
        {
            // Validate time range
            if (workHours.StartTime >= workHours.EndTime)
                throw new BadRequestException("Start time must be before end time.");

            var newWorkHours = await _workHoursRepository.AddAsync(workHours);
            await _dbTransactionContext.SaveChangesAsync();
            return newWorkHours;
        }
        catch
        {
            throw;
        }
    }

    public async Task<BarberWorkHoursModel> UpdateWorkHoursAsync(int workHourId, BarberWorkHoursModel workHours)
    {
        try
        {
            // Validate time range
            if (workHours.StartTime >= workHours.EndTime)
                throw new BadRequestException("Start time must be before end time.");

            var updatedWorkHours = await _workHoursRepository.UpdateAsync(workHourId, workHours);
            if (updatedWorkHours is null)
                throw new NotFoundException($"Work hours with ID {workHourId} not found.");

            await _dbTransactionContext.SaveChangesAsync();
            return updatedWorkHours;
        }
        catch
        {
            throw;
        }
    }

    public async Task<BarberWorkHoursModel> DeleteWorkHoursAsync(int workHourId)
    {
        try
        {
            var deletedWorkHours = await _workHoursRepository.RemoveByIdAsync(workHourId);
            if (deletedWorkHours is null)
                throw new NotFoundException($"Work hours with ID {workHourId} not found.");

            await _dbTransactionContext.SaveChangesAsync();
            return deletedWorkHours;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> IsBarberAvailableAsync(int barberId, DateTime appointmentDateTime, int durationMinutes = 30)
    {
        var dayOfWeek = (int)appointmentDateTime.DayOfWeek;
        var appointmentTime = TimeOnly.FromDateTime(appointmentDateTime);
        var appointmentEndTime = appointmentTime.AddMinutes(durationMinutes);

        var workHours = await _workHoursRepository.GetByBarberIdAndDayAsync(barberId, dayOfWeek);
        var activeWorkHours = workHours.Where(wh => wh.IsActive);

        foreach (var workHour in activeWorkHours)
        {
            // Check if appointment falls within work hours
            if (appointmentTime >= workHour.StartTime && appointmentEndTime <= workHour.EndTime)
            {
                return true;
            }
        }

        return false;
    }

    public async Task<IEnumerable<DateTime>> GetAvailableTimeSlotsAsync(int barberId, DateTime date, int durationMinutes = 30)
    {
        // Work entirely in UTC to avoid timezone issues
        // Convert input date to UTC if it's not already
        var utcDate = date.Kind == DateTimeKind.Utc ? date.Date : DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        var dayOfWeek = (int)utcDate.DayOfWeek;
        var workHours = await _workHoursRepository.GetByBarberIdAndDayAsync(barberId, dayOfWeek);
        var activeWorkHours = workHours.Where(wh => wh.IsActive);

        // Get existing appointments for this barber on this date (excluding cancelled)
        var existingAppointments = await _appointmentRepository.GetByBarberIdAndDateAsync(barberId, utcDate);
        var bookedSlots = existingAppointments
            .Where(a => a.Status != "Cancelled")
            .Select(a => a.AppointmentDate)
            .ToHashSet();

        var timeSlots = new List<DateTime>();

        foreach (var workHour in activeWorkHours)
        {
            var currentTime = workHour.StartTime;
            var endTime = workHour.EndTime;

            while (currentTime.AddMinutes(durationMinutes) <= endTime)
            {
                // Create UTC DateTime directly without timezone conversion
                var utcSlot = new DateTime(
                    utcDate.Year,
                    utcDate.Month,
                    utcDate.Day,
                    currentTime.Hour,
                    currentTime.Minute,
                    0,
                    DateTimeKind.Utc
                );

                // Only add if this slot is not already booked
                if (!bookedSlots.Contains(utcSlot))
                {
                    timeSlots.Add(utcSlot);
                }

                currentTime = currentTime.AddMinutes(durationMinutes);
            }
        }

        return timeSlots.OrderBy(ts => ts);
    }
}
