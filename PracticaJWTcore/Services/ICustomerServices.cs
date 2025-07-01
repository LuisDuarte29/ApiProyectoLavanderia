using ApiSwagger.Dtos;
using PracticaJWTcore.Entities;
using PracticaJWTcore.Models;
using System.Threading.Tasks;

namespace PracticaJWTcore.Services
{
    public interface ICustomerServices
    {

        Task<Customer> GetCustomer(long id);


        Task<Customer> CreateCustomer(CreateCustomerDto customersCreate);


        Task<bool> DeleteCustomers(long id);


        Task<IEnumerable<CustomerDto>> GetCustomerAll();
        Task<IEnumerable<CustomerDto>> UpdateCustomer(Customer customer);
    }
}
