using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public interface IUsuariosRepository
    {
        Task<IEnumerable<UsuarioDTO>> GetAllUsuarios();
        Task<bool> CreateUsuarios(CreateUsuariosDTO usuarioCreate);
        Task<IEnumerable<RoleModal>> GetAllRoles();
        Task<IEnumerable<RolesDTO>> GetRolesList();
        Task<IEnumerable<PermisosDTO>> PermisosRoleList(int roleId,string ComponentsId);
        Task<IEnumerable<PermisosModal>> GetPermisosList();
        Task<bool> PermisosRoleCreate(RolesPermisoDTO rolesPermisos);
        Task<IEnumerable<ComponentsForm>> GetComponentsForms();

    }
}
