
using TheNightOwls.Models;

namespace TheNightOwls.Repositories;

public interface ICustomerRepository : CustomerRepository
{
    Task<CustomerModel?> GetByIdAsync(CustomerModel customer);
    Task<IEnumerable<CustomerModel>> GetAll();
}
