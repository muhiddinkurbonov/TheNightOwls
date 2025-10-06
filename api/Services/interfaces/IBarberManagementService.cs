using Fadebook.Models;
namespace Fadebook.Services;

public interface IBarberManagementService
{
    Task<BarberModel?> GetByIdAsync(int id);
    Task<IEnumerable<BarberModel>> GetAllAsync();
    Task<BarberModel> AddAsync(BarberModel barber);
    Task<BarberModel> UpdateAsync(int barberId, BarberModel barber);
    Task<BarberModel> DeleteByIdAsync(int barberId);
    Task<IEnumerable<BarberServiceModel>> UpdateBarberServicesAsync(int barberId, List<int> selectedServiceIds);
}
