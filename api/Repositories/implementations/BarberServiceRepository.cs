

using Fadebook.DB;
using Fadebook.Models;
using Microsoft.EntityFrameworkCore;

namespace Fadebook.Repositories;

public class BarberServiceRepository : IBarberServiceRepository
{
    private readonly NightOwlsDbContext _nightOwlsDbContext;

    public BarberServiceRepository(NightOwlsDbContext nightOwlsDbContext)
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
        return await _nightOwlsDbContext.barberServiceTable
            .Where(bsm => bsm.BarberId == barberId && bsm.ServiceId == serviceId)
            .FirstAsync();
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
    public async Task<BarberServiceModel?> RemoveBarberServiceById(int barberServiceId)
    {
        // TODO: Throw exception for not found
        var barberSerice = await GetByIdAsync(barberServiceId);
        if (barberSerice == null) return null;
        // throw new NotFoundException($"There is no barber service with id: {barberServiceId}");
        _nightOwlsDbContext.barberServiceTable.Remove(barberSerice);
        return barberSerice;
    }
    public async Task<BarberServiceModel?> RemoveBarberServiceByBarberIdServiceId(int barberId, int serviceId)
    {
        // TODO: Throw exception for not found
        var foundBarberService = await this.GetBarberServiceByBarberIdServiceId(barberId, serviceId);
        if (foundBarberService == null) return null; // throw new NotFoundException($"There is no BarberService with barberId:{barberId}/serviceId{serviceId}");
        _nightOwlsDbContext.barberServiceTable.Remove(foundBarberService);
        return foundBarberService;
    }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _nightOwlsDbContext.SaveChangesAsync(cancellationToken);
    }
}
