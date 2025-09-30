
using Fadebook.Models;

namespace Fadebook.Repositories;

public interface IBarberRepository
{
    Task<BarberModel?> GetByIdAsync(BarberModel barber);
    Task<IEnumerable<BarberModel>> GetAll();
    Task <BarberModel> AddAsync(BarberModel barber);
    Task <BarberModel> UpdateAsync(BarberModel barber);
    Task <bool> DeleteByIdAsync(int id);
}
