
using Fadebook.DB;
using Fadebook.Models;
using Microsoft.EntityFrameworkCore;

namespace Fadebook.Repositories;

public class BarberServiceRespoitory: IBarberServiceRespoitory
{
    private readonly NightOwlsDbContext _nightOwlsDbContext;

    public BarberServiceRespoitory(NightOwlsDbContext nightOwlsDbContext)
    {
        _nightOwlsDbContext = nightOwlsDbContext;
    }

    public async Task<BarberServiceModel?> GetByIdAsync(int barberServiceId)
    {
        return await _nightOwlsDbContext.barberServiceTable.FindAsync(barberServiceId);
    }
    public async Task<IEnumerable<BarberServiceModel>> GetBarberServiceByBarberId(int barberId)
    {
        return await _nightOwlsDbContext.barberServiceTable
            .Where(bsm => bsm.BarberId == barberId)
            .ToListAsync();
    }
    public async Task<IEnumerable<BarberServiceModel>> GetBarberServiceByServiceId(int serviceId)
    {
        return await _nightOwlsDbContext.barberServiceTable
            .Where(bsm => bsm.ServiceId == serviceId)
            .ToListAsync();
        // select * from barberService where "ServiceId" == $serviceId
    }
    public async Task<BarberServiceModel?> GetBarberServiceByBarberIdServiceId(int barberId, int serviceId)
    {
        
    }
    public async Task<BarberServiceModel?> AddBarberService(int barberId, int servicerId)
    {
        // TODO: Get by, IF not null THEN return current 
        var foundBarberService = await this.GetBarberServiceByBarberIdServiceId(barberId, servicerId);
        // TODO: Throw exception to be handled -> throw, catch and return 40# code saying resource exists
        if (foundBarberService != null) return foundBarberService;
        BarberServiceModel barberServiceModel = new BarberServiceModel();
        barberServiceModel.BarberId = barberId;
        barberServiceModel.ServiceId = servicerId;
        await _nightOwlsDbContext.barberServiceTable.AddAsync(barberServiceModel);
        return barberServiceModel;
    }
    Task<BarberServiceModel?> RemoveBarberServiceById(int barberServiceId)
    {
        
    }
    public async Task<BarberServiceModel?> RemoveBarberServiceByBarberIdServiceId(int barberId, int serviceId)
    {
        throw new NotImplementedException();
    }
}
