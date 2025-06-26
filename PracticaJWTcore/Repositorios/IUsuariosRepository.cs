using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public interface IUsuariosRepository
    {
        Task<List<UsuarioDTO>> GetAllUsuarios();
        Task<bool> CreateUsuarios(CreateUsuariosDTO usuarioCreate);
        Task<List<RoleModal>> GetAllRoles();
        Task<List<RolesDTO>> GetRolesList();
        Task<List<PermisosDTO>> PermisosRoleList(int roleId,int ComponentsId);
        Task<List<PermisosModal>> GetPermisosList();
        Task<bool> PermisosRoleCreate(RolesPermisoDTO rolesPermisos);
        Task<List<ComponentsForm>> GetComponentsForms();

    }
}
