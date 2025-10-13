using Fadebook.Models;

namespace Fadebook.Services;
public interface IBarberWorkHoursService
{
    Task<IEnumerable<BarberWorkHoursModel>> GetAllWorkHoursAsync();
    Task<BarberWorkHoursModel> GetWorkHoursByIdAsync(int workHourId);
    Task<IEnumerable<BarberWorkHoursModel>> GetWorkHoursByBarberIdAsync(int barberId);
    Task<IEnumerable<BarberWorkHoursModel>> GetWorkHoursByBarberIdAndDayAsync(int barberId, int dayOfWeek);
    Task<BarberWorkHoursModel> AddWorkHoursAsync(BarberWorkHoursModel workHours);
    Task<BarberWorkHoursModel> UpdateWorkHoursAsync(int workHourId, BarberWorkHoursModel workHours);
    Task<BarberWorkHoursModel> DeleteWorkHoursAsync(int workHourId);
    Task<bool> IsBarberAvailableAsync(int barberId, DateTime appointmentDateTime, int durationMinutes = 30);
    Task<IEnumerable<DateTime>> GetAvailableTimeSlotsAsync(int barberId, DateTime date, int durationMinutes = 30);
}
