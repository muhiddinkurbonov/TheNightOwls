using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fadebook.Models;
using Fadebook.Repositories;
using Moq;
using Xunit;

namespace Api.Tests.Repositories
{
    public class CustomerRepositoryTests
    {
        //
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;

        public CustomerRepositoryTests()
        {
            _mockCustomerRepository = new Mock<ICustomerRepository>();
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


            _mockCustomerRepository
              .Setup(r => r.GetByIdAsync(customerId))
                .ReturnsAsync(customer);

            // Act
            var result = await _mockCustomerRepository.Object.GetByIdAsync(customerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customerId, result.CustomerId);
        }
    }
}