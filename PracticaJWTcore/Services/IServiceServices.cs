using PracticaJWTcore.Models;

namespace PracticaJWTcore.Services
{
    public interface IServiceServices
    {

        Task<Service> GetService(long id);


        Task<Service> CreateService(Service services);

        Task<bool> DeleteService(long id);

        Task<List<Service>> GetServiceAll();

        Task<List<Service>> UpdateServices(Service services);
    }
}
