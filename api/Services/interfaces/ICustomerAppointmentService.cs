
using Fadebook.Models;

namespace Fadebook.Services;

public interface ICustomerAppointmentService
{




    // requestAppointment
    Task<AppointmentModel> RequestAppointmentAsync(CustomerModel customer, AppointmentModel appointment);



    //getBarberByService
    Task<IEnumerable<BarberModel>> GetBarbersByServiceAsync(int serviceId);



    //getServices/*
    Task<IEnumerable<ServiceModel>> GetServicesAsync();


}
