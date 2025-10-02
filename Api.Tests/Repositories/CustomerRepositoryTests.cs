using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fadebook.Models;
using Fadebook.Repositories;
using Fadebook.Api.Tests.TestUtilities;
using Moq;
using Xunit;

namespace Api.Tests.Repositories
{
    public class CustomerRepositoryTests: RepositoryTestBase
    {
        //
        private readonly ICustomerRepository _mockCustomerRepository;

        public CustomerRepositoryTests()
        {
            _mockCustomerRepository = new CustomerRepository(_context);
        }


        [Fact]
        public async Task GetCustomerById_ShouldReturnCustomer_WhenCustomerExists()
        {
            // Arrange
            int customerId = 1;
            var customer = new CustomerModel
            {
                CustomerId = customerId,
                Name = "Jane Smith",
                Username = "janesmith",
                ContactInfo = "555-5678"
            };

            // Act
            var result = await _mockCustomerRepository.AddCustomerAsync(customer);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customerId, result.CustomerId);
        }
    }
}