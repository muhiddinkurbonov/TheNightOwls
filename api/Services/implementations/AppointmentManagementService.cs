
using Fadebook.DB;
using Fadebook.Models;
using Fadebook.Repositories;
using Fadebook.Exceptions;


namespace Fadebook.Services;

public class AppointmentManagementService : IAppointmentManagementService
{
    private readonly NightOwlsDbContext _dbContext;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IBarberRepository _barberRepository;

    public AppointmentManagementService(
        NightOwlsDbContext dbContext,
        IAppointmentRepository appointmentRepository,
        ICustomerRepository customerRepository,
        IBarberRepository barberRepository
        )
    {
        _dbContext = dbContext;
        _appointmentRepository = appointmentRepository;
        _customerRepository = customerRepository;
        _barberRepository = barberRepository;
    }

    public async Task<AppointmentModel?> AddAppointment(AppointmentModel appointmentModel)
    {
        var foundAppointment = await _appointmentRepository.GetByIdAsync(appointmentModel.AppointmentId);
        if (foundAppointment != null) return foundAppointment;
        return await _appointmentRepository.AddAppointment(appointmentModel);
    }
    public async Task<AppointmentModel?> GetAppointmentById(int appointmentId)
    {
        return await _appointmentRepository.GetByIdAsync(appointmentId);
    }

    public async Task<AppointmentModel?> UpdateAppointment(AppointmentModel appointment)
    {
        var foundAppointment = await _appointmentRepository.GetByIdAsync(appointment.AppointmentId);
        if (foundAppointment == null) return null;
        var updated = await _appointmentRepository.UpdateAppointment(appointment);
        return updated;
    }
    public async Task<IEnumerable<AppointmentModel>> GetAppointmentsByDate(DateTime dateTime)
    {
        return await _appointmentRepository.GetApptsByDate(dateTime);
    }
    public async Task<AppointmentModel?> DeleteAppointment(AppointmentModel appointment)
    {
        var deleted = await _appointmentRepository.DeleteApptById(appointment.AppointmentId);
        return deleted;
    }
    public async Task<IEnumerable<AppointmentModel>?> LookupAppointmentsByUsername(string username)
    {
        var foundCustomer = await _customerRepository.GetByUsernameAsync(username);
        if (foundCustomer == null) return null;
        return await _appointmentRepository.GetByCustomerId(foundCustomer.CustomerId);
    }

    /*
        Task<AppointmentModel?> GetByIdAsync(int appointmentId);
        Task<IEnumerable<AppointmentModel>> GetAll();
        Task<IEnumerable<AppointmentModel>> GetApptsByDate(DateTime dateTime);
        Task<IEnumerable<AppointmentModel>> GetByCustomerId(int customerId);
        Task<AppointmentModel> AddAppointment(AppointmentModel appointmentModel);
        Task<AppointmentModel> UpdateAppointment(AppointmentModel appointmentModel);
        Task<AppointmentModel> DeleteApptById(int appointmentId);
    */
}
