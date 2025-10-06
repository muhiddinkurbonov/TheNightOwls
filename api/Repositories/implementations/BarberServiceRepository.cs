

using Fadebook.DB;
using Fadebook.Models;
using Microsoft.EntityFrameworkCore;

namespace Fadebook.Repositories;

public class BarberServiceRepository(
    FadebookDbContext _fadebookDbContext
    ) : IBarberServiceRepository
{
    public async Task<BarberServiceModel?> GetByIdAsync(int barberServiceId)
    {
        return await _fadebookDbContext.barberServiceTable.FindAsync(barberServiceId);
    }
    public async Task<IEnumerable<BarberServiceModel>> GetByBarberIdAsync(int barberId)
    {
        return await _fadebookDbContext.barberServiceTable
            .Where(bsm => bsm.BarberId == barberId)
            .ToListAsync();
    }
    public async Task<IEnumerable<BarberServiceModel>> GetByServiceIdAsync(int serviceId)
    {
        return await _fadebookDbContext.barberServiceTable
            .Where(bsm => bsm.ServiceId == serviceId)
            .ToListAsync();
    }
    public async Task<BarberServiceModel?> GetByBarberIdServiceIdAsync(int barberId, int serviceId)
    {
        return await _fadebookDbContext.barberServiceTable
            .Where(bsm => bsm.BarberId == barberId && bsm.ServiceId == serviceId)
            .FirstAsync();
    }
    public async Task<BarberServiceModel> AddAsync(BarberServiceModel barberServiceModel)
    {
        var foundBarberService = await this.GetByBarberIdServiceIdAsync(barberServiceModel.BarberId, barberServiceModel.ServiceId);
        if (foundBarberService != null)
            throw new InvalidOperationException($"BarberService with BarberId {barberServiceModel.BarberId} and ServiceId {barberServiceModel.ServiceId} already exists.");
        await _fadebookDbContext.barberServiceTable.AddAsync(barberServiceModel);
        return barberServiceModel;
    }
    public async Task<BarberServiceModel> RemoveByIdAsync(int barberServiceId)
    {
        var foundBarberService = await this.GetByIdAsync(barberServiceId);
        if (foundBarberService is null)
            throw new KeyNotFoundException($"BarberService with BarberServiceId {barberServiceId}");
        _fadebookDbContext.barberServiceTable.Remove(foundBarberService);
        return foundBarberService;
    }
    public async Task<BarberServiceModel> RemoveByBarberIdServiceId(int barberId, int serviceId)
    {
        var foundBarberService = await this.GetByBarberIdServiceIdAsync(barberId, serviceId);
        if (foundBarberService is null)
            throw new KeyNotFoundException($"BarberService with BarberId {barberId} and ServiceId {serviceId} not found.");
        _fadebookDbContext.barberServiceTable.Remove(foundBarberService);
        return foundBarberService;
    }
}
