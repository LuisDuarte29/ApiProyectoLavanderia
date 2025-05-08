using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public interface ICustomerModal
    {
        Task<List<CustomerModal>> CustomerModalGetAll();
    }
}
