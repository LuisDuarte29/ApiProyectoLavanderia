using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Services
{
    public interface IUsuarioServices
    {
        Task<List<UsuarioDTO>> GetAllUsuarios();
        Task<bool> CreateUsuarios(CreateUsuariosDTO usuarioCreate);
        Task<List<RoleModal>> GetAllRoles();
        Task<List<RolesDTO>> GetRolesList();
        Task<List<PermisosModal>> GetPermisosList();
        Task<List<PermisosDTO>> PermisosRoleList(int rolId);
        Task<bool> PermisosRoleCreate(RolesPermisoDTO rolesPermisos);
        Task<List<ComponentsForm>> GetComponentsForms();
    }
}
