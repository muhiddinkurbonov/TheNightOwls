
// TODO: G3

using Fadebook.DB;
using Fadebook.Models;
using Microsoft.EntityFrameworkCore;

namespace Fadebook.Repositories;

public class ServiceRepository: IServiceRepository
{
    private readonly NightOwlsDbContext _NightOwlsDbContext;

    public ServiceRepository(NightOwlsDbContext nightOwlsDbContext)
    {
        _NightOwlsDbContext = nightOwlsDbContext;
    }


    public async Task<ServiceModel?> GetByIdAsync(int serviceId)
    {
        return await _NightOwlsDbContext.serviceTable
            .FindAsync(serviceId);
    }
    public async Task<IEnumerable<ServiceModel>> GetAll()
    {
        return await _NightOwlsDbContext.serviceTable.ToListAsync();
    }
}
