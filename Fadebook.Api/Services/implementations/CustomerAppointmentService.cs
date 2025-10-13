
using Fadebook.DB;
using Fadebook.Models;
using Fadebook.Repositories;
using Fadebook.Exceptions;
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
            throw new BadRequestException($"Provide a complete appointment model\n{appointmentModel.ToJson()}");
        if (string.IsNullOrEmpty(appointmentModel.Status))
            throw new BadRequestException($"Provide a complete appointment model\n{appointmentModel.ToJson()}");

        // Check for double-booking: ensure the barber doesn't have an existing non-cancelled appointment at this time
        var appointmentDate = appointmentModel.AppointmentDate;
        var existingAppointments = await _appointmentRepository.GetByBarberIdAndDateAsync(
            appointmentModel.BarberId,
            appointmentDate
        );

        var conflictingAppointment = existingAppointments.FirstOrDefault(a =>
            a.AppointmentDate == appointmentDate &&
            a.Status != "Cancelled"
        );

        if (conflictingAppointment != null)
        {
            throw new BadRequestException(
                $"This time slot is already booked. Please select a different time."
            );
        }

        try
        {
            var appointment = await _appointmentRepository.AddAsync(appointmentModel);
            if (appointment is null)
                throw new BadRequestException("Unable to create appointment. Verify that Customer, Barber, and Service IDs exist.");
            await _dbTransactionContext.SaveChangesAsync();
            return appointment;
        }
        catch
        {
            throw;
        }
    }
}
