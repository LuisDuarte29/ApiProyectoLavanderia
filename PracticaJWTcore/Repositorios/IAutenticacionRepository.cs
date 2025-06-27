

using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public interface IAutenticacionRepository
    {
        Task<TokenRolDTO> GenerateToken(UsuarioLogin usuario,int result, int rolId,string rolName);
        Task<TokenRolDTO> Login(UsuarioLogin usuario);
        Task<int> CambioClave(CambioClave cambios);
       
    }
}
