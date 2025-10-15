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
        // Get the business timezone from environment variable (default to America/Chicago for Central Time)
        var timezoneId = Environment.GetEnvironmentVariable("BUSINESS_TIMEZONE") ?? "America/Chicago";
        TimeZoneInfo businessTimeZone;

        try
        {
            // Try IANA timezone ID (works on Linux/Mac)
            businessTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
        }
        catch
        {
            // Fallback to Windows timezone ID
            try
            {
                var windowsId = timezoneId == "America/Chicago" ? "Central Standard Time" : timezoneId;
                businessTimeZone = TimeZoneInfo.FindSystemTimeZoneById(windowsId);
            }
            catch
            {
                // If all else fails, use UTC
                businessTimeZone = TimeZoneInfo.Utc;
            }
        }

        // Get the date in the business timezone
        var utcNow = DateTime.UtcNow;
        var businessNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, businessTimeZone);

        // The incoming date parameter should be treated as a date in the business timezone, not UTC
        // Extract just the date components and create a new DateTime in the business timezone
        var businessDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Unspecified);

        var dayOfWeek = (int)businessDate.DayOfWeek;
        var workHours = await _workHoursRepository.GetByBarberIdAndDayAsync(barberId, dayOfWeek);
        var activeWorkHours = workHours.Where(wh => wh.IsActive);

        // Get existing appointments for this barber on this date (excluding cancelled)
        var existingAppointments = await _appointmentRepository.GetByBarberIdAndDateAsync(barberId, businessDate.Date);
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
                // Create datetime in business timezone
                var businessSlot = new DateTime(
                    businessDate.Year,
                    businessDate.Month,
                    businessDate.Day,
                    currentTime.Hour,
                    currentTime.Minute,
                    0,
                    DateTimeKind.Unspecified
                );

                // Convert to UTC for transmission to frontend
                var utcSlot = TimeZoneInfo.ConvertTimeToUtc(businessSlot, businessTimeZone);

                // Skip slots that are in the past
                // For today, only show slots that are at least 30 minutes in the future
                if (businessDate.Date == businessNow.Date)
                {
                    if (businessSlot <= businessNow.AddMinutes(30))
                    {
                        currentTime = currentTime.AddMinutes(durationMinutes);
                        continue;
                    }
                }

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
