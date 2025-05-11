
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public interface IServicioModal
    {
        Task<List<ServiciosModal>> ServiciosModalsGetAll();
    }
}
