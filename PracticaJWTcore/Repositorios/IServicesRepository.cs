using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public interface IServicesRepository
    {

        Task<Service> GetService(long id);


        Task<Service> CreateService(Service services);

        Task<bool> DeleteService(long id);

        Task<IEnumerable<Service>> GetServiceAll();

        Task<IEnumerable<Service>> UpdateServices(Service services);

        Task<IEnumerable<Articulos>> GetAllArticulos();

    }
}
