using Fadebook.Models;

namespace Fadebook.Repositories;

public interface IBarberWorkHoursRepository
{
    Task<IEnumerable<BarberWorkHoursModel>> GetAllAsync();
    Task<BarberWorkHoursModel?> GetByIdAsync(int workHourId);
    Task<IEnumerable<BarberWorkHoursModel>> GetByBarberIdAsync(int barberId);
    Task<IEnumerable<BarberWorkHoursModel>> GetByBarberIdAndDayAsync(int barberId, int dayOfWeek);
    Task<BarberWorkHoursModel> AddAsync(BarberWorkHoursModel workHours);
    Task<BarberWorkHoursModel?> UpdateAsync(int workHourId, BarberWorkHoursModel workHours);
    Task<BarberWorkHoursModel?> RemoveByIdAsync(int workHourId);
    Task<bool> IsBarberAvailableAsync(int barberId, DateTime appointmentDateTime, int durationMinutes = 30);
}
