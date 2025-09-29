
using TheNightOwls.Models;

namespace TheNightOwls.Repositories;

public interface IBarberRepository
{
    Task<BarberModel?> GetByIdAsync(BarberModel barber);
    Task<IEnumerable<BarberModel>> GetAll();
}
