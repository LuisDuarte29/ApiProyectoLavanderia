
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Services
{
    public interface IAutenticacionServices
    {
        Task<int> CambioClave(string nuevaClave, string correo);
        Task<string> Login(UsuarioLogin usuario);
    }
}
