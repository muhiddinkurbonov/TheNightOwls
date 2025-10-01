
using Fadebook.DB;
using Fadebook.Models;
using Microsoft.EntityFrameworkCore;

namespace Fadebook.Services;

public class CustomerAppointmentService : ICustomerAppointmentService
{

    private readonly NightOwlsDbContext _db;

    public CustomerAppointmentService(NightOwlsDbContext db)
    {
        _db = db;
    }

   
    public async Task<AppointmentModel> RequestAppointmentAsync(
        int customerId,
        int barberId,
        int serviceId,
        string username,
        DateTime scheduledAt)
    {
        // 1. Check if related rows exist
        var customer = await _db.customerTable.FindAsync(customerId);
        if (customer == null)
            throw new Exception("Customer not found.");

        var barber = await _db.barberTable.FindAsync(barberId);
        if (barber == null)
            throw new Exception("Barber not found.");

        var service = await _db.serviceTable.FindAsync(serviceId);
        if (service == null)
            throw new Exception("Service not found.");

        //  Verify that this barber actually offers this service via BarberServiceModel
        bool barberOffersService = await _db.barberServiceTable
            .AnyAsync(bs => bs.BarberId == barberId && bs.ServiceId == serviceId);

        if (!barberOffersService)
            throw new Exception("This barber does not offer the selected service.");

        // 3. Create the appointment entity
        var appointment = new AppointmentModel
        {
            Username = username,
            Status = scheduledAt,
            CustomerId = customerId,
            BarberId = barberId,
            ServiceId = serviceId,
            Customer = customer,
            Barber = barber,
            Service = service
        };

        // 4. Save to the database
        _db.appointmentTable.Add(appointment);
        await _db.SaveChangesAsync();

        return appointment;
    }



    //GetBarbersByServiceAsync
    public async Task<IEnumerable<BarberModel>> GetBarbersByServiceAsync(int serviceId)
    {
        var barbers = await _db.barberServiceTable
            .Where(bs => bs.ServiceId == serviceId)
            .Include(bs => bs.Barber)
            .Select(bs => bs.Barber)  // Select only the Barber entities
            .ToListAsync();

        return barbers;
    }


    //GetServicesAsync
    public async Task<IEnumerable<ServiceModel>> GetServicesAsync()
    {
        return await _db.serviceTable.ToListAsync();
    }

    
}
