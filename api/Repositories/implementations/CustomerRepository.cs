
using TheNightOwls.Models;

namespace TheNightOwls.Repositories;

public class CustomerRepository: ICustomerRepository
{
    public async Task<CustomerModel?> GetByIdAsync(CustomerModel customer)
    {
        throw new NotImplementedException();
    }
    public async Task<IEnumerable<CustomerModel>> GetAll()
    {
        throw new NotImplementedException();
    }
}
