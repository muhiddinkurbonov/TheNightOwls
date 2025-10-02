using Fadebook.Models;
namespace Fadebook.Services;

public interface IBarberManagementService
{
    public Task<BarberModel?> GetByIdAsync(int id);
    public Task<IEnumerable<BarberModel?>> GetAllAsync();
    public Task<BarberModel> AddAsync(BarberModel barber);
    public Task<BarberModel?> UpdateAsync(BarberModel barber);
    public Task<bool> DeleteByIdAsync(int id);
    public Task<bool> UpdateBarberServicesAsync(int barberId, List<int> serviceIds);
}
