
using TheNightOwls.Models;
using TheNightOwls.DB;
using Microsoft.EntityFrameworkCore;


namespace TheNightOwls.Repositories;

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
    public Task<CustomerModel?> GetByUsername(string username)
    {
        throw new NotImplementedException();
    }

}
