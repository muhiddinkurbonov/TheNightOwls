
// TODO: G2
using Fadebook.Models;
using Fadebook.DB;
using Microsoft.EntityFrameworkCore;


using Fadebook.Models;

namespace Fadebook.Repositories;

public class CustomerRepository: ICustomerRepository
{

    private readonly NightOwlsDbContext _db;

        //Constructor Injection
     public CustomerRepository(NightOwlsDbContext db)
        {
            _db = db;
        }

    //find customer by id
    public async Task<CustomerModel?> GetByIdAsync(int id)
        {
            return await _db.customerTable.FindAsync(id);
        }

    //get all customers
    public async Task<IEnumerable<CustomerModel>> GetAllAsync()
    {
        return await _db.customerTable.ToListAsync();
    }

    //find customer by username
    public async Task<CustomerModel?> GetByUsernameAsync(string username)
    {
        return await _db.customerTable.FirstOrDefaultAsync(c => c.Username == username);
    }

}
