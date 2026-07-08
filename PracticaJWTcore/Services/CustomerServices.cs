using ApiSwagger.Dtos;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PracticaJWTcore.Entities;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;

namespace PracticaJWTcore.Services
{
    // Service de clientes: capa intermedia para mantener controllers desacoplados del repository.
    public class CustomerServices : ICustomerServices
    {
        private readonly ICustomerRepository _context;
        public CustomerServices(ICustomerRepository context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<Customer?> GetCustomer(long id)
        {
            return await _context.GetCustomer(id);
        }
        public async Task<Customer> CreateCustomer(CreateCustomerDto customersCreate)
        {
            return await _context.CreateCustomer(customersCreate);
        }
        public async Task<ServiceResult<object>> DeleteCustomers(long id)
        {
            var customer = await _context.GetCustomer(id);
            if (customer == null)
                return ServiceResult<object>.Fail("Cliente no encontrado", "CUSTOMER_NOT_FOUND");

            if (await _context.CustomerHasVentasOrUsuarios(id))
                return ServiceResult<object>.Fail("No se puede eliminar el cliente porque tiene ventas o usuarios asociados", "CUSTOMER_HAS_DEPENDENCIES");

            await _context.DeleteCustomers(id);
            return ServiceResult<object>.Ok(new object());
        }

        public async Task<IEnumerable<CustomerDto>> GetCustomerAll()
        {
            return await _context.GetCustomerAll();
        }

        public Task<IEnumerable<CustomerDto>> UpdateCustomer(Customer customer)
        {
            return _context.UpdateCustomer(customer);
        }
    }
}
