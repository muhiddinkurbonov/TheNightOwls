
using Fadebook.DB;
using Fadebook.Models;
using Fadebook.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fadebook.Services;

public class CustomerAppointmentService(
    IDbTransactionContext _dbTransactionContext,
    IServiceRepository _serviceRepository,
    IBarberServiceRepository _barberServiceRepository,
    IBarberRepository _barberRepository,
    IAppointmentRepository _appointmentRepository
    ) : ICustomerAppointmentService
{
    public async Task<IEnumerable<ServiceModel>> ListAvailableServicesAsync()
    {
        return await _serviceRepository.GetAll();
    }

    public async Task<IEnumerable<BarberModel>> ListAvailableBarbersByServiceAsync(int serviceId)
    {
        var barberServices = await _barberServiceRepository.GetByServiceIdAsync(serviceId);
        return barberServices.Select(bsm => bsm.Barber);
    }

    public async Task<IEnumerable<AppointmentModel>> GetAppointmentsByCustomerIdAsync(int customerId)
    {
        return await _appointmentRepository.GetByCustomerIdAsync(customerId);
    }

    public async Task<AppointmentModel> MakeAppointmentAsync(AppointmentModel appointmentModel)
    {
        // Validate required fields
        if (appointmentModel.CustomerId == 0 || appointmentModel.ServiceId == 0 || appointmentModel.BarberId == 0)
            throw new InvalidOperationException($"Provide a complete appointment model\n{appointmentModel.ToJson()}");
        if (string.IsNullOrEmpty(appointmentModel.Status))
            throw new InvalidOperationException($"Provide a complete appointment model\n{appointmentModel.ToJson()}");
        try
        {
            var appointment = await _appointmentRepository.AddAsync(appointmentModel);
            await _dbTransactionContext.SaveChangesAsync();
            return appointment;
        }
        catch
        {
            throw;
        }
    }
}
