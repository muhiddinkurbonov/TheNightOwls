
using Fadebook.Models;

namespace Fadebook.Repositories;

public interface IBarberServiceRepository
{
    Task<BarberServiceModel?> GetByIdAsync(int barberServiceId);
    Task<IEnumerable<BarberServiceModel>> GetBarberServiceByBarberId(int barberId);
    Task<IEnumerable<BarberServiceModel>> GetBarberServiceByServiceId(int serviceId);
    Task<BarberServiceModel?> GetBarberServiceByBarberIdServiceId(int barberId, int serviceId);
    Task<BarberServiceModel?> AddBarberService(int barberId, int serviceId);
    Task<BarberServiceModel?> RemoveBarberServiceById(int barberServiceId);
    Task<BarberServiceModel?> RemoveBarberServiceByBarberIdServiceId(int barberId, int serviceId);
}
