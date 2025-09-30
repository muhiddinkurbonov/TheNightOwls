
using Fadebook.Models;

namespace Fadebook.Repositories;

public interface ICustomerRepository
{
    Task<CustomerModel?> GetByIdAsync(CustomerModel customer);
    Task<IEnumerable<CustomerModel>> GetAll();
}
