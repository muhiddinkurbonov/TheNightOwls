
// TODO: G2

using Fadebook.Models;

namespace Fadebook.Repositories;

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
