
using TheNightOwls.Models;

namespace TheNightOwls.Repositories;

public interface IAppointmentRepository
{
    Task<AppointmentModel?> GetByIdAsync(AppointmentModel appointment);
    Task<IEnumerable<AppointmentModel>> GetAll();
}
