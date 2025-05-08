using PracticaJWTcore.Models;
using PracticaJWTcore.Dtos;
using PracticaJWTcore.Repositorios;
using Microsoft.EntityFrameworkCore;

namespace PracticaJWTcore.Services
{
    public class CustomerModalServices : ICustomerModal
    {
        public readonly PracticaJWTcoreContext _context;
        public CustomerModalServices (PracticaJWTcoreContext context)
        {
            _context = context;
        }
        public async Task<List<CustomerModal>> CustomerModalGetAll()
        {
            return await _context.Customer.Select(x => new CustomerModal
            {
                CustomerId = x.Id,
                NombreCustomer=x.FirstName
            }).ToListAsync();
        }
    }
}
