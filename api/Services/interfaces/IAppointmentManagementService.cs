using Fadebook.Models;

namespace Fadebook.Services;


public interface IAppointmentManagementService
{
    Task<AppointmentModel?> AddAppointment(AppointmentModel appointmentModel);
    Task<AppointmentModel?> GetAppointmentById(int appointmentId);
    Task<AppointmentModel?> UpdateAppointment(AppointmentModel appointment);
    Task<IEnumerable<AppointmentModel>> GetAppointmentsByDate(DateTime dateTime);
    Task<AppointmentModel?> DeleteAppointment(AppointmentModel appointment);
    Task<IEnumerable<AppointmentModel>?> LookupAppointmentsByUsername(string username);
}
