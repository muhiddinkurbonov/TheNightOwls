
using Fadebook.Models;

namespace Fadebook.Services;

public interface ICustomerAppointmentService
{


   

    // requestAppointment
    Task<AppointmentModel> RequestAppointmentAsync(
            int customerId,
            int barberId,
            int serviceId,
            string username,
            DateTime scheduledAt);



    //getBarberByService
    Task<IEnumerable<BarberModel>> GetBarbersByServiceAsync(int serviceId);



    //getServices/*
    Task<IEnumerable<ServiceModel>> GetServicesAsync();


}
