using ApiSwagger.Dtos;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Entities;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public interface ICustomerRepository
    {

        Task<Customer> GetCustomer(long id);


       Task<Customer> CreateCustomer(CreateCustomerDto customersCreate);


         Task<bool> DeleteCustomers(long id);


        Task<IEnumerable<CustomerDto>> GetCustomerAll();
        Task<IEnumerable<CustomerDto>> UpdateCustomer(Customer customer);

    }
}
