
using Fadebook.Models;
using Fadebook.DB;
using Microsoft.EntityFrameworkCore;

namespace Fadebook.Repositories;

public class BarberRepository(
    FadebookDbContext _fadebookDbContext
    ) : IBarberRepository
{
    public async Task<BarberModel?> GetByIdAsync(int id)
    {
        return await _fadebookDbContext.barberTable.FindAsync(id);
    }

    public async Task<IEnumerable<BarberModel>> GetAllAsync()
    {
        return await _fadebookDbContext.barberTable.ToListAsync();
    }

    public async Task<BarberModel> AddAsync(BarberModel barber)
    {
        var result = await _fadebookDbContext.barberTable.AddAsync(barber);
        return result.Entity;
    }

    public async Task<BarberModel> UpdateAsync(int barberId, BarberModel barberModel)
    {

        var foundBarberModel = await GetByIdAsync(barberId);
        if (foundBarberModel is null)
            throw new KeyNotFoundException($"Barber with ID {barberId} was not found.");
        foundBarberModel.Update(barberModel);
        _fadebookDbContext.barberTable.Update(foundBarberModel);
        return foundBarberModel;
    }

    public async Task<BarberModel> RemoveByIdAsync(int barberId)
    {
        var foundBarberModel = await GetByIdAsync(barberId);
        if (foundBarberModel is null)
            throw new KeyNotFoundException($"Barber with ID {barberId} was not found.");

        _fadebookDbContext.barberTable.Remove(foundBarberModel);
        return foundBarberModel;
    }
}
