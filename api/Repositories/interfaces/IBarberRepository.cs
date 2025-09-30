
using Fadebook.Models;

namespace Fadebook.Repositories;

public interface IBarberRepository
{
    Task<BarberModel?> GetByIdAsync(BarberModel barber);
    Task<IEnumerable<BarberModel>> GetAll();
}
