

using Fadebook.Models;
using Fadebook.DB;
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

    public async Task<BarberModel> AddAsync(BarberModel barber)
    {
        var result = await _context.barberTable.AddAsync(barber);
        //await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<BarberModel> UpdateAsync(BarberModel barber)
    {
        // return await _context.barberTable.UpdateAsync(barber);
        // if (await _context.Barbers.FindAsync(id) is null) throw new InvalidOperationException("*Barber with ID " + id + " not found*");
        //     barber.Id = id;

        //     var existing = await _context.Barbers.FindAsync(id);
        //     _context.Entry(existing).CurrentValues.SetValues(barber);
        //     await _context.SaveChangesAsync();
        
        var existing = await _context.barberTable.FirstOrDefaultAsync(b => b.BarberId == barber.BarberId);
        if (existing is null) return null;

        existing.Username = barber.Username;
        existing.Name = barber.Name;
        existing.Specialty = barber.Specialty;
        existing.ContactInfo = barber.ContactInfo;

        // Do not attach a second instance; the tracked 'existing' has been mutated.
        //await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteByIdAsync(int id)
    {
        var entity = await _context.barberTable.FindAsync(id);
        if (entity is null) return false;

        _context.barberTable.Remove(entity);
        //await _context.SaveChangesAsync();
        return true;
    }
}
