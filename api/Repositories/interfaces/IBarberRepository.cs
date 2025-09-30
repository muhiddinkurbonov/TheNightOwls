
using TheNightOwls.Models;

namespace TheNightOwls.Repositories;

public interface IBarberRepository
{
    Task<BarberModel?> GetByIdAsync(BarberModel barber);
    Task<IEnumerable<BarberModel>> GetAll();
    Task <BarberModel> AddAsync(BarberModel barber);
    Task <BarberModel> UpdateAsync(BarberModel barber);
    Task <BarberModel> DeleteAsync(BarberModel barber);
}
