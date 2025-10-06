using Microsoft.EntityFrameworkCore;
using Fadebook.Models;
using Fadebook.DB;

namespace Fadebook.Repositories;

public class CustomerRepository(
    FadebookDbContext _fadebookDbContext
    ) : ICustomerRepository
{
    //find customer by id
    public async Task<CustomerModel?> GetByIdAsync(int id)
    {
        return await _fadebookDbContext.customerTable.FindAsync(id);
    }

    //get all customers
    public async Task<IEnumerable<CustomerModel>> GetAllAsync()
    {
        return await _fadebookDbContext.customerTable.ToListAsync();
    }

    // TODO: Add Customer

    //find customer by username
    public async Task<CustomerModel?> GetByUsernameAsync(string username)
    {
        return await _fadebookDbContext.customerTable.Where(c => c.Username == username).FirstAsync();
    }

    public async Task<CustomerModel> UpdateAsync(int customerId, CustomerModel customer)
    {
        var foundCustomerModel = await GetByIdAsync(customerId);
        if (foundCustomerModel is null)
            throw new KeyNotFoundException($"Customer with CustomerId {customerId} was not found");
        if (customer.Username != null && foundCustomerModel.Username != customer.Username)
        {
            var usernameCustomerModel = GetByUsernameAsync(customer.Username);
            if (usernameCustomerModel != null)
                throw new InvalidOperationException($"Customer with username {customer.Username} already exists.");
        }
        foundCustomerModel.Update(customer);
        _fadebookDbContext.customerTable.Update(foundCustomerModel);
        return foundCustomerModel;
    }

    public async Task<CustomerModel> AddAsync(CustomerModel customer)
    {
        var usernameCustomerModel = GetByUsernameAsync(customer.Username);
        if (usernameCustomerModel != null)
            throw new InvalidOperationException($"Customer with username {customer.Username} already exists.");
        await _fadebookDbContext.customerTable.AddAsync(customer);
        return customer;
    }
}
