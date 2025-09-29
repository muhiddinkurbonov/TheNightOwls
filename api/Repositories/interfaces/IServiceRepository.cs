
using TheNightOwls.Models;

namespace TheNightOwls.Repositories;

public interface IServiceRepository
{
    Task<ServiceModel?> GetByIdAsync(ServiceModel service);
    Task<IEnumerable<ServiceModel>> GetAll();
}
