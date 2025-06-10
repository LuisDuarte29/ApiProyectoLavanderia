
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Services
{
    public interface IAutenticacionServices
    {
        Task<int> CambioClave(CambioClave cambio);
        Task<string> Login(UsuarioLogin usuario);
    }
}
