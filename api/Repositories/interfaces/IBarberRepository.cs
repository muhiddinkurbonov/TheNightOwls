
using Fadebook.Models;

namespace Fadebook.Repositories;

public interface IBarberRepository
{
    Task<BarberModel?> GetByIdAsync(int id);
    Task<IEnumerable<BarberModel>> GetAllAsync();
    Task <BarberModel> AddAsync(BarberModel barber);
    Task <BarberModel?> UpdateAsync(BarberModel barber);
    Task <bool> DeleteByIdAsync(int id);
}
