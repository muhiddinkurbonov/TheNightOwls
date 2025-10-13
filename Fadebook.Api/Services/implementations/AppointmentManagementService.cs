
using Fadebook.DB;
using Fadebook.Models;
using Fadebook.Repositories;
using Fadebook.Exceptions;


namespace Fadebook.Services;

public class AppointmentManagementService(
    IDbTransactionContext _dbTransactionContext,
    IAppointmentRepository _appointmentRepository,
    ICustomerRepository _customerRepository
    ) : IAppointmentManagementService
{
    public async Task<IEnumerable<AppointmentModel>> GetAllAppointmentsAsync()
    {
        return await _appointmentRepository.GetAllAsync();
    }

    public async Task<AppointmentModel> AddAppointmentAsync(AppointmentModel appointmentModel)
    {
        try
        {
            Console.WriteLine($"[DEBUG] Adding appointment - CustomerId: {appointmentModel.CustomerId}, BarberId: {appointmentModel.BarberId}, ServiceId: {appointmentModel.ServiceId}");

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

            var newAppointment = await _appointmentRepository.AddAsync(appointmentModel);
            if (newAppointment is null)
                throw new BadRequestException("Unable to create appointment. Verify that Customer, Barber, and Service IDs exist.");

            Console.WriteLine($"[DEBUG] Appointment added, saving changes...");
            await _dbTransactionContext.SaveChangesAsync();
            Console.WriteLine($"[DEBUG] Changes saved, appointment ID: {newAppointment.AppointmentId}");

            // Reload the appointment with navigation properties for proper DTO mapping
            Console.WriteLine($"[DEBUG] Reloading appointment with navigation properties...");
            var reloadedAppointment = await _appointmentRepository.GetByIdAsync(newAppointment.AppointmentId);
            Console.WriteLine($"[DEBUG] Appointment reloaded successfully");

            return reloadedAppointment ?? newAppointment;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to add appointment: {ex.Message}");
            Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
            throw;
        }
    }
    public async Task<AppointmentModel> UpdateAppointmentAsync(int appointmentId, AppointmentModel appointmentModel)
    {
        try
        {
            var updatedAppointment = await _appointmentRepository.UpdateAsync(appointmentId, appointmentModel);
            if (updatedAppointment is null)
                throw new NotFoundException($"Appointment with ID {appointmentId} not found or invalid foreign keys.");
            await _dbTransactionContext.SaveChangesAsync();
            return updatedAppointment;
        }
        catch
        {
            throw;
        }
    }
    public async Task<IEnumerable<AppointmentModel>> GetAppointmentsByDateAsync(DateTime dateTime)
    {
        return await _appointmentRepository.GetByDateAsync(dateTime);
    }
    public async Task<IEnumerable<AppointmentModel>> GetAppointmentsByCustomerIdAsync(int customerId)
    {
        return await _appointmentRepository.GetByCustomerIdAsync(customerId);
    }
    public async Task<AppointmentModel> GetAppointmentByIdAsync(int appointmentId)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if (appointment is null)
            throw new NotFoundException($"Appointment with id \"{appointmentId} does not exist");
        return appointment; 
    }
    public async Task<IEnumerable<AppointmentModel>> GetAppointmentsByBarberIdAsync(int barberId)
    {
        return await _appointmentRepository.GetByBarberIdAsync(barberId);
    }
    public async Task<AppointmentModel> DeleteAppointmentAsync(int appointmentId)
    {
        try
        {
            var removedAppointment = await _appointmentRepository.RemoveByIdAsync(appointmentId);
            if (removedAppointment is null)
                throw new NotFoundException($"Appointment with ID {appointmentId} not found.");
            await _dbTransactionContext.SaveChangesAsync();
            return removedAppointment;
        }
        catch
        {
            throw;
        }
    }
    // Throws NoUsernameException
    public async Task<IEnumerable<AppointmentModel>> LookupAppointmentsByUsernameAsync(string username)
    {
        var foundCustomer = await _customerRepository.GetByUsernameAsync(username);
        if (foundCustomer == null)
            throw new NotFoundException($"Customer with the username \"{username}\" was not found");
        return await _appointmentRepository.GetByCustomerIdAsync(foundCustomer.CustomerId);
    }
}
