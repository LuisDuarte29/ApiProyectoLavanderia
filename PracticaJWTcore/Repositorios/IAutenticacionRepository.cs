

using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public interface IAutenticacionRepository
    {
        Task<string> GenerateToken(UsuarioLogin usuario, List<string> roles, int result);
        Task<string> Login(UsuarioLogin usuario);
        Task<int> CambioClave(string nuevaClave, string correo);
       
    }
}
