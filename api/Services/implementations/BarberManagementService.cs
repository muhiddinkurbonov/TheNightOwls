using Fadebook.Models;
using Fadebook.Repositories;

namespace Fadebook.Services;


public class BarberManagementService(
    IDbTransactionContext _dbTransactionContext,
    IBarberRepository _barberRepository,
    IBarberServiceRepository _barberServiceRepository
    ) : IBarberManagementService
{
    public async Task<BarberModel?> GetByIdAsync(int id)
    {
        var barberEntity = await _barberRepository.GetByIdAsync(id);
        return barberEntity;
    }

    public async Task<IEnumerable<BarberModel>> GetAllAsync()
    {
        return await _barberRepository.GetAllAsync();
    }
    public async Task<BarberModel> AddAsync(BarberModel barber)
    {
        try
        {
            var newBarber = await _barberRepository.AddAsync(barber);
            await _dbTransactionContext.SaveChangesAsync();
            return newBarber;
        }
        catch
        {
            throw;
        }
    }
    public async Task<BarberModel> UpdateAsync(int barberId, BarberModel barber)
    {
        try
        {
            var updatedBarber = await _barberRepository.UpdateAsync(barberId, barber);
            await _dbTransactionContext.SaveChangesAsync();
            return updatedBarber;
        }
        catch
        {
            throw;
        }
    }
    public async Task<BarberModel> DeleteByIdAsync(int barberId)
    {
        try
        {
            var deletedBarber = await _barberRepository.RemoveByIdAsync(barberId);
            await _dbTransactionContext.SaveChangesAsync();
            return deletedBarber;
        }
        catch
        {
            // Nothing for us to correct here
            // So we pass the exception on.
            // Also, apparently this is how we rethrow an expception per CS2200
            // Manually rethrowing an exception results in a stack trace rewrite causing lost info.
            throw;
        }
    }
    // Update a barber's available services
    public async Task<IEnumerable<BarberServiceModel>> UpdateBarberServicesAsync(int barberId, List<int> selectedServiceIds)
    {
        // Remove all current services for the barber
        try
        {
            var barberServices = await _barberServiceRepository.GetByBarberIdAsync(barberId);
            IEnumerable<int> barberServiceIds = barberServices.Select(bsm => bsm.ServiceId);
            IEnumerable<int> addServiceIds = selectedServiceIds.Except(barberServiceIds).ToList();
            IEnumerable<int> removeServiceIds = barberServiceIds.Except(selectedServiceIds).ToList();
            foreach (int addServiceId in addServiceIds)
            {
                await _barberServiceRepository.AddAsync(new BarberServiceModel { BarberId=barberId, ServiceId=addServiceId });
            }
            foreach (int removeServiceId in removeServiceIds)
            {
                await _barberServiceRepository.RemoveByBarberIdServiceId(barberId, removeServiceId);
            }
            await _dbTransactionContext.SaveChangesAsync();
            return await _barberServiceRepository.GetByBarberIdAsync(barberId);
        }
        catch
        {
            // Nothing for us to correct here
            // So we pass the exception on.
            // Also, apparently this is how we rethrow an expception per CS2200
            // Manually rethrowing an exception results in a stack trace rewrite causing lost info.
            throw;
        }
    }
}
