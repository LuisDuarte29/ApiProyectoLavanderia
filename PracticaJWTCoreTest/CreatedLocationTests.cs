using ApiSwagger.Dtos;
using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Controllers;
using PracticaJWTcore.Dtos;
using PracticaJWTcore.Entities;
using PracticaJWTcore.Models;
using PracticaJWTcore.Services;

namespace PracticaJWTCoreTest;

public class CreatedLocationTests
{
    // Estas pruebas aseguran que CreatedResult conserve Locations relativas compatibles.
    [Fact]
    public async Task CreateCustomer_no_devuelve_localhost_hardcodeado()
    {
        var controller = new CustomerController(new FakeCustomerServices());

        var result = await controller.CreateCustomer(new CreateCustomerDto
        {
            FirstName = "Luis",
            LastName = "Duarte",
            Email = "luis@example.com",
            Phone = "123",
            Address = "Calle 1"
        });

        var created = Assert.IsType<CreatedResult>(result);
        Assert.Equal("api/customer/8", created.Location);
    }

    private sealed class FakeCustomerServices : ICustomerServices
    {
        public Task<Customer> CreateCustomer(CreateCustomerDto customersCreate) => Task.FromResult(new Customer { Id = 8 });
        public Task<ServiceResult<object>> DeleteCustomers(long id) => Task.FromResult(ServiceResult<object>.Ok(new object()));
        public Task<Customer?> GetCustomer(long id) => Task.FromResult<Customer?>(new Customer { Id = id });
        public Task<IEnumerable<CustomerDto>> GetCustomerAll() => Task.FromResult(Enumerable.Empty<CustomerDto>());
        public Task<IEnumerable<CustomerDto>> UpdateCustomer(Customer customer) => Task.FromResult(Enumerable.Empty<CustomerDto>());
    }

}
