
using TheNightOwls.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TheNightOwls.Repositories;

public interface ICustomerRepository 
{
    Task<CustomerModel?> GetByIdAsync(int id);
    Task<IEnumerable<CustomerModel>> GetAllAsync();
    Task<CustomerModel?> GetByUsernameAsync(string username);

    
}
