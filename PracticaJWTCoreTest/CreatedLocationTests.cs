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
    public async Task CreateAppointment_no_devuelve_localhost_hardcodeado()
    {
        var controller = new AppointmentController(new FakeAppointmentServices());

        var result = await controller.CreateAppointment(new CreateAppoitmentDetailsDTO());

        var created = Assert.IsType<CreatedResult>(result);
        Assert.Equal("api/Appointment/7", created.Location);
    }

    [Fact]
    public async Task CreateCustomer_no_devuelve_localhost_hardcodeado()
    {
        var controller = new CustomerController(new FakeCustomerServices());

        var result = await controller.CreateCustomer(new CreateCustomerDto());

        var created = Assert.IsType<CreatedResult>(result);
        Assert.Equal("api/customer/8", created.Location);
    }

    [Fact]
    public async Task CreateService_no_devuelve_localhost_hardcodeado()
    {
        var controller = new ServiceController(new FakeServiceServices());

        var result = await controller.CreateServices(new Service { ServiceId = 0, ServiceName = "Lavado" });

        var created = Assert.IsType<CreatedResult>(result);
        Assert.Equal("api/Service/9", created.Location);
    }

    private sealed class FakeAppointmentServices : IAppointmentServices
    {
        public Task<long> CreateAppointment(CreateAppoitmentDetailsDTO appointment) => Task.FromResult(7L);
        public Task<bool> DeleteApointment(long id) => Task.FromResult(true);
        public Task<AppoitmentDTO> GetAppointment(long id) => Task.FromResult(new AppoitmentDTO());
        public Task<IEnumerable<AppoitmentDTO>> GetAppointmentAll() => Task.FromResult(Enumerable.Empty<AppoitmentDTO>());
        public Task<IEnumerable<AppoitmentDTO>> UpdateAppointment(UpdateAppoitmentDetailsDTO appointment) => Task.FromResult(Enumerable.Empty<AppoitmentDTO>());
    }

    private sealed class FakeCustomerServices : ICustomerServices
    {
        public Task<Customer> CreateCustomer(CreateCustomerDto customersCreate) => Task.FromResult(new Customer { Id = 8 });
        public Task<bool> DeleteCustomers(long id) => Task.FromResult(true);
        public Task<Customer> GetCustomer(long id) => Task.FromResult(new Customer { Id = id });
        public Task<IEnumerable<CustomerDto>> GetCustomerAll() => Task.FromResult(Enumerable.Empty<CustomerDto>());
        public Task<IEnumerable<CustomerDto>> UpdateCustomer(Customer customer) => Task.FromResult(Enumerable.Empty<CustomerDto>());
    }

    private sealed class FakeServiceServices : IServiceServices
    {
        public Task<Service> CreateService(Service services)
        {
            services.ServiceId = 9;
            return Task.FromResult(services);
        }

        public Task<bool> DeleteService(long id) => Task.FromResult(true);
        public Task<IEnumerable<Articulos>> GetArticulos() => Task.FromResult(Enumerable.Empty<Articulos>());
        public Task<Service> GetService(long id) => Task.FromResult(new Service { ServiceId = id });
        public Task<IEnumerable<Service>> GetServiceAll() => Task.FromResult(Enumerable.Empty<Service>());
        public Task<IEnumerable<Service>> UpdateServices(Service services) => Task.FromResult(Enumerable.Empty<Service>());
    }
}
