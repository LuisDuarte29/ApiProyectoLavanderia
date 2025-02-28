
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Services
{
    public interface IAutenticacionServices
    {

        Task<string> Login(UsuarioLogin usuario);
    }
}
