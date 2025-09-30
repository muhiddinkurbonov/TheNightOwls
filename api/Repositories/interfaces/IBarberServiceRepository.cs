
using Fadebook.Models;

namespace Fadebook.Repositories;

public interface IBarberServiceRespoitory
{
    Task<BarberServiceModel?> GetByIdAsync(BarberServiceModel barberService);
    Task<IEnumerable<BarberServiceModel>> GetAll();
}
