
using TheNightOwls.Models;

namespace TheNightOwls.Repositories;

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
