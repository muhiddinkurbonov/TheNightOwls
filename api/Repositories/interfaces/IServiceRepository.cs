
using Fadebook.Models;

namespace Fadebook.Repositories;

public interface IServiceRepository
{
    Task<ServiceModel?> GetByIdAsync(ServiceModel service);
    Task<IEnumerable<ServiceModel>> GetAll();
}
