
using Fadebook.Models;
using TheNightOwls.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


<<<<<<< HEAD
public interface ICustomerRepository : CustomerRepository
=======
namespace Fadebook.Repositories;

public interface ICustomerRepository 
>>>>>>> 46687dc91493a82cc6225fb94406eda9e760483f
{
    Task<CustomerModel?> GetByIdAsync(int id);
    Task<IEnumerable<CustomerModel>> GetAllAsync();
    Task<CustomerModel?> GetByUsernameAsync(string username);

    
}
