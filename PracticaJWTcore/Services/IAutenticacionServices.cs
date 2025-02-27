
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Services
{
    public interface IAutenticacionServices
    {

        Task<string> Login(Usuario usuario);
    }
}
