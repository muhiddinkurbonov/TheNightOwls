
using Fadebook.Models;

namespace Fadebook.Repositories;

public interface IAppointmentRepository: DbSaveChanges
{
    Task<AppointmentModel?> GetByIdAsync(int appointmentId);
    Task<IEnumerable<AppointmentModel>> GetAll();
    Task<IEnumerable<AppointmentModel>> GetApptsByDate(DateTime dateTime);
    Task<IEnumerable<AppointmentModel>> GetByCustomerId(int customerId);
    Task<AppointmentModel> AddAppointment(AppointmentModel appointmentModel);
    Task<AppointmentModel> UpdateAppointment(AppointmentModel appointmentModel);
    Task<AppointmentModel> DeleteApptById(int appointmentId);
}
