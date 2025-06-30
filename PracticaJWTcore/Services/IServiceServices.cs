using PracticaJWTcore.Models;

namespace PracticaJWTcore.Services
{
    public interface IServiceServices
    {

        Task<Service> GetService(long id);


        Task<Service> CreateService(Service services);

        Task<bool> DeleteService(long id);

        Task<IEnumerable<Service>> GetServiceAll();

        Task<IEnumerable<Service>> UpdateServices(Service services);
        Task<IEnumerable<Articulos>> GetArticulos();
    }
}
