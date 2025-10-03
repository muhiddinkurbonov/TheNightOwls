
using Fadebook.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Fadebook.Repositories;

public interface ICustomerRepository: DbSaveChanges
{
    Task<CustomerModel?> GetByIdAsync(int id);
    Task<IEnumerable<CustomerModel>> GetAllAsync();
    Task<CustomerModel?> GetByUsernameAsync(string username);
    // TODO:
    Task<CustomerModel?> UpdateCustomerAsync(CustomerModel customer);
    Task<CustomerModel> AddCustomerAsync(CustomerModel customer);
}
