
using Fadebook.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


<<<<<<< HEAD
public interface ICustomerRepository : CustomerRepository
=======
namespace Fadebook.Repositories;

<<<<<<< HEAD
public interface ICustomerRepository 
>>>>>>> 46687dc91493a82cc6225fb94406eda9e760483f
=======
public interface ICustomerRepository
>>>>>>> d09cb30d0c0deb36c87aff9da5f1a5884887a920
{
    Task<CustomerModel?> GetByIdAsync(int id);
    Task<IEnumerable<CustomerModel>> GetAllAsync();
    Task<CustomerModel?> GetByUsernameAsync(string username);
    // TODO:
    Task<CustomerModel?> UpdateCustomerAsync(CustomerModel customer);
}
