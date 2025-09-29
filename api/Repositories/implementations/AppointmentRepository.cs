
using TheNightOwls.Models;

namespace TheNightOwls.Repositories;

public class AppointmentRepository: IAppointmentRepository
{
    public async Task<AppointmentModel?> GetByIdAsync(AppointmentModel appointment)
    {
        throw new NotImplementedException();
    }
    public async Task<IEnumerable<AppointmentModel>> GetAll()
    {
        throw new NotImplementedException();
    }
}
