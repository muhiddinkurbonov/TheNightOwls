
using TheNightOwls.Models;

namespace TheNightOwls.Repositories;

public interface IBarberServiceRespoitory
{
    Task<BarberServiceModel?> GetByIdAsync(BarberServiceModel barberService);
    Task<IEnumerable<BarberServiceModel>> GetAll();
}
