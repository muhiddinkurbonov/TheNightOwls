

using TheNightOwls.Models;
using TheNightOwls.DB;
using Microsoft.EntityFrameworkCore;

namespace Fadebook.Repositories;

public class BarberRepository: IBarberRepository
{
    private readonly NightOwlsDbContext _context;

    public BarberRepository(NightOwlsDbContext context)
    {
        _context = context;
    }

    public async Task<BarberModel?> GetByIdAsync(BarberModel barber)
    {
        return await _context.barberTable.FirstOrDefaultAsync(b => b.BarberId == barber.BarberId);
    }

    public async Task<IEnumerable<BarberModel>> GetAll()
    {
        return await _context.barberTable.ToListAsync();
    }

    public async Task<BarberModel> AddAsync()
    {
        return await _context.barberTable.AddAsync(barber);

    }

    public async Task<BarberModel> UpdateAsync();
    {
        return await _context.barberTable.UpdateAsync(barber);
    }

    public async Task<BarberModel> DeleteAsync(BarberModel barber);
    {
        return await _context.barberTable.RemoveAsync(barber);
    }
}
