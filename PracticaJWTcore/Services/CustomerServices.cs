using ApiSwagger.Dtos;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PracticaJWTcore.Entities;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;

namespace PracticaJWTcore.Services
{
    public class CustomerServices : ICustomerServices
    {
        private readonly ICustomerRepository _context;
        public CustomerServices(ICustomerRepository context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<Customer> GetCustomer(long id)
        {
            return await _context.GetCustomer(id);
        }
        public async Task<Customer> CreateCustomer(CreateCustomerDto customersCreate)
        {
            return await _context.CreateCustomer(customersCreate);
        }
        public async Task<bool> DeleteCustomers(long id)
        {
            return await _context.DeleteCustomers(id);
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
