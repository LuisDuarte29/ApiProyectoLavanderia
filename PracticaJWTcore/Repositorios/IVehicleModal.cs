
using PracticaJWTcore.Models;
namespace PracticaJWTcore.Repositorios

{
    public interface IVehicleModal
    {
         Task<List<VehicleModal>> VehicleModalGetAll();
  

    }
}
