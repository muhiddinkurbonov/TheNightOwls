using Microsoft.EntityFrameworkCore;
using TheNightOwls.Models;
using TheNightOwls.DB;


namespace TheNightOwls.Repositories;

public class CustomerRepository : ICustomerRepository
{
    public async Task<CustomerModel?> GetByIdAsync(CustomerModel customer)
    {
        return await _context.Customers
        .AsNoTracking()
        .FirstOrDefaultAsync();
        throw new NotImplementedException();
    }
    public async Task<IEnumerable<CustomerModel>> GetAll()
    {
        throw new NotImplementedException();
    }
    public async Task<CustomerModel> UpdateAsync(CustomerModel entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        var existing = await _context.Customers.FindAsync(customer.CustomerId);
    }
    
}
