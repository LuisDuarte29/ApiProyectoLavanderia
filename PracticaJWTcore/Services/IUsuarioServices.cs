using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Services
{
    public interface IUsuarioServices
    {
        Task<List<UsuarioDTO>> GetAllUsuarios();
        Task<bool> CreateUsuarios(CreateUsuariosDTO usuarioCreate);
        Task<List<RoleModal>> GetAllRoles();
    }
}
