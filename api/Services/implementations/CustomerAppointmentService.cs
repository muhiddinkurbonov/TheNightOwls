
using Fadebook.DB;
using Fadebook.Models;
using Fadebook.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fadebook.Services;

public class CustomerAppointmentService : ICustomerAppointmentService
{

    private readonly NightOwlsDbContext _db;
    private readonly ICustomerRepository _customerRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IBarberServiceRepository _barberServiceRepository;
    private readonly IBarberRepository _barberRepository;
    private readonly IAppointmentRepository _appointmentRepository;

    public CustomerAppointmentService(
        NightOwlsDbContext db,
        ICustomerRepository customerRepository,
        IServiceRepository serviceRepository,
        IBarberServiceRepository barberServiceRepository,
        IBarberRepository barberRepository,
        IAppointmentRepository appointmentRepository
        )
    {
        _db = db;
        _customerRepository = customerRepository;
        _serviceRepository = serviceRepository;
        _barberRepository = barberRepository;
        _barberServiceRepository = barberServiceRepository;
        _appointmentRepository = appointmentRepository;
    }

   
    public async Task<AppointmentModel> RequestAppointmentAsync(CustomerModel customer, AppointmentModel appointment)
    {
        // 1. Check if related rows exist
        // var customer = await _db.customerTable.FindAsync(customerId);

        // TODO: Validate appointment data

        // TODO: Check for valid barber
        // var barber = await _db.barberTable.FindAsync(barberId);
        // if (barber == null)
        //     throw new Exception("Barber not found.");

        // TODO: Check for valid service
        // var service = await _db.serviceTable.FindAsync(serviceId);
        // if (service == null)
        //     throw new Exception("Service not found.");

        //  Verify that this barber actually offers this service via BarberServiceModel
        // bool barberOffersService = await _db.barberServiceTable
        //     .AnyAsync(bs => bs.BarberId == barberId && bs.ServiceId == serviceId);

        // if (!barberOffersService)
        //     throw new Exception("This barber does not offer the selected service.");

        // 3. Create the appointment entity
        // var appointment = new AppointmentModel
        // {
        //     Username = username,
        //     Status = scheduledAt,
        //     CustomerId = customerId,
        //     BarberId = barberId,
        //     ServiceId = serviceId,
        //     Customer = customer,
        //     Barber = barber,
        //     Service = service
        // };

        customer = await _customerRepository.AddCustomerAsync(customer);
        // 4. Save to the database
        appointment.Customer = customer;
        appointment.CustomerId = customer.CustomerId;
        _db.appointmentTable.Add(appointment);
        await _db.SaveChangesAsync();

        return appointment;
    }



    //GetBarbersByServiceAsync
    public async Task<IEnumerable<BarberModel>> GetBarbersByServiceAsync(int serviceId)
    {
        // var barbers = await _db.barberServiceTable
        //     .Where(bs => bs.ServiceId == serviceId)
        //     .Include(bs => bs.Barber)
        //     .Select(bs => bs.Barber)  // Select only the Barber entities
        //     .ToListAsync();
        IEnumerable<BarberServiceModel> foundBarberServices = await _barberServiceRepository.GetBarberServiceByServiceId(serviceId);
        IEnumerable<BarberModel> foundBarbers = foundBarberServices.Select(bs => bs.Barber);
        return foundBarbers;
    }


    //GetServicesAsync
    public async Task<IEnumerable<ServiceModel>> GetServicesAsync()
    {
        return await _serviceRepository.GetAll();
    }
    //GetCustomerByIdAsync
    public async Task<CustomerModel?> GetCustomerByIdAsync(int customerId)
    {
        return await _customerRepository.GetByIdAsync(customerId);
    }

    //AddCustomer
    public async Task<CustomerModel> AddCustomerAsync(CustomerModel customer)
    {
        return await _customerRepository.AddCustomerAsync(customer);
    }
    
    
}
