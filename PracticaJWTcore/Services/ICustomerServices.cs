using ApiSwagger.Dtos;
using PracticaJWTcore.Entities;
using PracticaJWTcore.Models;
using System.Threading.Tasks;

namespace PracticaJWTcore.Services
{
    public interface ICustomerServices
    {

        Task<CustomerEntity> GetCustomer(long id);


        Task<CustomerEntity> CreateCustomer(CreateCustomerDto customersCreate);


        Task<bool> DeleteCustomers(long id);


        Task<List<CustomerDto>> GetCustomerAll();
        Task<List<CustomerDto>> UpdateCustomer(Customer customer);
    }
}
