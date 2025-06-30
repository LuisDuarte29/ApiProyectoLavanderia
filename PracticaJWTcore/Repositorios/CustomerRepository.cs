using ApiSwagger.Dtos;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Entities;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public class CustomerRepository:ICustomerRepository
    {
        private readonly PracticaJWTcoreContext _context;
        public CustomerRepository(PracticaJWTcoreContext context)
        {
            _context = context;
        }


        public async Task<CustomerEntity> GetCustomer(long id)
        {
            return await _context.CustomerEntity.FirstAsync(x => x.Id == id);
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
            EntityEntry<CustomerEntity> response = await _context.CustomerEntity.AddAsync(customerEntity);
            await _context.SaveChangesAsync();
            return await GetCustomer(response.Entity.Id);
        }
        public async Task<bool> DeleteCustomers(long id)
        {
            CustomerEntity customerEntity = await GetCustomer(id);
            _context.CustomerEntity.Remove(customerEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CustomerDto>> GetCustomerAll()
        {
            return await _context.CustomerEntity.Select(x => x.ToDto()).ToListAsync();
        }

        public async Task<IEnumerable<CustomerDto>> UpdateCustomer(Customer customer)
        {
            CustomerEntity customerEntity = await _context.CustomerEntity.FirstAsync(x=>x.Id==customer.Id);
            customerEntity.FirstName = customer.FirstName;
            customerEntity.Email = customer.Email;
            customerEntity.Phone = customer.Phone;
            customerEntity.Address = customer.Address;
            await _context.SaveChangesAsync();
            return await GetCustomerAll();
            
        }
    }
}
