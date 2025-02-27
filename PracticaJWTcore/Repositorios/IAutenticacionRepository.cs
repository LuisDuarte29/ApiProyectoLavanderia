

using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public interface IAutenticacionRepository
    {
        Task<string> GenerateToken(Usuario usuario, List<string> roles, int result);
        Task<string> Login(Usuario usuario);
       
    }
}
