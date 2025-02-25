using ApiSwagger.Dtos;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PracticaJWTcore.Entities;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;

namespace PracticaJWTcore.Services
{
    public class CustomerServices
    {
        private readonly CustomerDataBaseCustomer _context;
        public CustomerServices(CustomerDataBaseCustomer context)
        {
            _context = context;
        }


        public async Task<CustomerEntity> GetCustomer(long id)
        {
            return await _context.Customers.FirstAsync(x => x.Id == id);
        }
        public async Task<CustomerEntity> CreateCustomer(CreateCustomerDto customersCreate)
        {
            CustomerEntity customerEntity = new CustomerEntity
            {
                Id = 0,
                FirstName = customersCreate.FirstName,
                Email = customersCreate.Email,
                Phone = customersCreate.Phone,
                Address = customersCreate.Address

            };
            //El EntryEntity nos permite hacer un seguimiento de los cambios en la entidad
            EntityEntry<CustomerEntity> response = await _context.Customers.AddAsync(customerEntity);
            await _context.SaveChangesAsync();
            return await GetCustomer(response.Entity.Id);
        }
        public async Task<bool> DeleteCustomers(long id)
        {
            CustomerEntity customerEntity = await GetCustomer(id);
            _context.Customers.Remove(customerEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CustomerDto>> GetCustomerAll()
        {
            return await _context.Customers.Select(x=>x.ToDto()).ToListAsync();
        }
    }
}
