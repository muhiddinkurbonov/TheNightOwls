
using Fadebook.Models;

namespace Fadebook.Repositories;

public interface IServiceRepository: DbSaveChanges
{
    Task<ServiceModel?> GetByIdAsync(int serviceId);
    Task<IEnumerable<ServiceModel>> GetAll();
}

/*


*/