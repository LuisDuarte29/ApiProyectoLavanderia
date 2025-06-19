

using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public interface IAutenticacionRepository
    {
        Task<string> GenerateToken(UsuarioLogin usuario, string roles, int result);
        Task<string> Login(UsuarioLogin usuario);
        Task<int> CambioClave(CambioClave cambios);
       
    }
}
