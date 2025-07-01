using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Services
{
    public interface IUsuarioServices
    {
        Task<IEnumerable<UsuarioDTO>> GetAllUsuarios();
        Task<bool> CreateUsuarios(CreateUsuariosDTO usuarioCreate);
        Task<IEnumerable<RoleModal>> GetAllRoles();
        Task<IEnumerable<RolesDTO>> GetRolesList();
        Task<IEnumerable<PermisosModal>> GetPermisosList();
        Task<IEnumerable<PermisosDTO>> PermisosRoleList(int rolId, string componentsId);
        Task<bool> PermisosRoleCreate(RolesPermisoDTO rolesPermisos);
        Task<IEnumerable<ComponentsForm>> GetComponentsForms();

        Task<IEnumerable<PermisosDTO>> PermisosRoleListAsignacion(int rolId, int componentsId);
    }
}
