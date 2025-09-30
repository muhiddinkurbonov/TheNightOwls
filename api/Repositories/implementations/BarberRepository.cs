
// G1

using Fadebook.Models;

namespace Fadebook.Repositories;

public class BarberRepository: IBarberRepository
{
    public async Task<BarberModel?> GetByIdAsync(BarberModel barber)
    {
        throw new NotImplementedException();
    }
    public async Task<IEnumerable<BarberModel>> GetAll()
    {
        throw new NotImplementedException();
    }
}
