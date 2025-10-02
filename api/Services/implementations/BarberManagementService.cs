using Fadebook.Models;
using Fadebook.Repositories;

namespace Fadebook.Services;


public class BarberManagementService(BarberRepository repo) : IBarberManagementService
{
    public async Task<BarberModel?> GetByIdAsync(int id)
    {
        var barberEntity = await repo.GetByIdAsync(id);
        if (barberEntity is null) return null;
        return barberEntity;
    }

    public async Task<IEnumerable<BarberModel>> GetAllAsync()
    {
        return await repo.GetAllAsync();
    }
    public async Task<BarberModel> AddAsync(BarberModel barber)
    {
        var result = await repo.AddAsync(barber);
        await repo.SaveChangesAsync();
        return result;
    }
    public async Task<BarberModel?> UpdateAsync(BarberModel barber)
    {
        var updatedBarber = await repo.UpdateAsync(barber);
        if (updatedBarber is null) return null;
        await repo.SaveChangesAsync();
        return updatedBarber;
    }
    public async Task<bool> DeleteByIdAsync(int id)
    {
        var result = await repo.DeleteByIdAsync(id);
        await repo.SaveChangesAsync();
        return result;
    }
    // TODO: Update a barbers available services: Add and remove linked BarberService services 
}
