
using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Services
{
    public interface IAutenticacionServices
    {
        Task<int> CambioClave(CambioClave cambio);
        Task<TokenRolDTO> Login(UsuarioLogin usuario);
    }
}
