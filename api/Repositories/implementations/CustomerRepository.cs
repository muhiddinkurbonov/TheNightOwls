<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;
using TheNightOwls.Models;
using TheNightOwls.DB;

=======

// TODO: G2
using Fadebook.Models;
using Fadebook.DB;
using Microsoft.EntityFrameworkCore;
>>>>>>> 46687dc91493a82cc6225fb94406eda9e760483f


using Fadebook.Models;

namespace Fadebook.Repositories;

public class CustomerRepository : ICustomerRepository
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
<<<<<<< HEAD
        return await _context.Customers
        .AsNoTracking()
        .FirstOrDefaultAsync();
        throw new NotImplementedException();
=======
        return await _db.customerTable.ToListAsync();
>>>>>>> 46687dc91493a82cc6225fb94406eda9e760483f
    }

    //find customer by username
    public async Task<CustomerModel?> GetByUsernameAsync(string username)
    {
        return await _db.customerTable.FirstAsync(c => c.Username == username);
    }

    // TODO: update customer
    public async Task<CustomerModel?> UpdateCustomerAsync(CustomerModel customer)
    {
        var existingCustomer = await _db.customerTable.FindAsync(customer.CustomerId);
        
        if (existingCustomer == null)
        {
            return null;
        }

        // Update the properties of the existing customer
        existingCustomer.Username = customer.Username;
        existingCustomer.Name = customer.Name;
        existingCustomer.ContactInfo = customer.ContactInfo;

        await _db.SaveChangesAsync();
        return existingCustomer;
        
    }
<<<<<<< HEAD
    public async Task<CustomerModel> UpdateAsync(CustomerModel entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        var existing = await _context.Customers.FindAsync(customer.CustomerId);
    }
    
=======

>>>>>>> 46687dc91493a82cc6225fb94406eda9e760483f
}
