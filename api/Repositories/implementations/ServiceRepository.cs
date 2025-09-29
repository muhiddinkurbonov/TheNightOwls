
using TheNightOwls.Models;

namespace TheNightOwls.Repositories;

public class ServiceRepository: IServiceRepository
{
    public async Task<ServiceModel?> GetByIdAsync(ServiceModel service)
    {
        throw new NotImplementedException();
    }
    public async Task<IEnumerable<ServiceModel>> GetAll()
    {
        throw new NotImplementedException();
    }
}
