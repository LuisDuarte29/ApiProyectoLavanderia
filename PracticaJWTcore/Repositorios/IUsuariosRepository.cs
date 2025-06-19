using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public interface IUsuariosRepository
    {
        Task<List<UsuarioDTO>> GetAllUsuarios();
      Task<bool> CreateUsuarios(CreateUsuariosDTO usuarioCreate);

        Task<List<RoleModal>> GetAllRoles();
    }
}
