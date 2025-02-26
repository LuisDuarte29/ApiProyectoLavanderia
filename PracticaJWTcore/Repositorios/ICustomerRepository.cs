using ApiSwagger.Dtos;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Entities;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public interface ICustomerRepository
    {

        Task<CustomerEntity> GetCustomer(long id);


       Task<CustomerEntity> CreateCustomer(CreateCustomerDto customersCreate);


         Task<bool> DeleteCustomers(long id);


        Task<List<CustomerDto>> GetCustomerAll();
        Task<List<CustomerDto>> UpdateCustomer(Customer customer);

    }
}
