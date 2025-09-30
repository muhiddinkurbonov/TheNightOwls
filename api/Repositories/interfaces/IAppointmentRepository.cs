
using Fadebook.Models;

namespace Fadebook.Repositories;

public interface IAppointmentRepository
{
    Task<AppointmentModel?> GetByIdAsync(AppointmentModel appointment);
    Task<IEnumerable<AppointmentModel>> GetAll();
}
