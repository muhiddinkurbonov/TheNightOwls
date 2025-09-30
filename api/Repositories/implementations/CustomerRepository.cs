using Microsoft.EntityFrameworkCore;
using Fadebook.Models;
using Fadebook.DB;



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
        return await _db.customerTable.ToListAsync();
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
    public async Task<CustomerModel> UpdateAsync(CustomerModel customerModel)
    {
        var foundCustomerModel = await GetByIdAsync(customerModel.CustomerId);
        if (foundCustomerModel is null) return null;
        customerModel.CustomerId = foundCustomerModel.CustomerId;
        _db.customerTable.Update(customerModel);
        return customerModel;
    }
    
}
