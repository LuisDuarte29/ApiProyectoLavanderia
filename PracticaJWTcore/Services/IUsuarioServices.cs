using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Services
{
    public interface IUsuarioServices
    {
        Task<IEnumerable<UsuarioDTO>> GetAllUsuarios();
        Task<bool> CreateUsuarios(CreateUsuariosDTO usuarioCreate);
        Task<ServiceResult<RolesDTO>> CreateRole(CreateRoleDTO dto);
        Task<ServiceResult<RolesDTO>> UpdateRole(int roleId, UpdateRoleDTO dto);
        Task<IEnumerable<RoleModal>> GetAllRoles();
        Task<IEnumerable<RolesDTO>> GetRolesList();
        Task<IEnumerable<PermisosModal>> GetPermisosList();
        Task<IEnumerable<PermisosDTO>> PermisosRoleList(int rolId, string componentsId);
        Task<bool> PermisosRoleCreate(RolesPermisoDTO rolesPermisos);
        Task<IEnumerable<ComponentsForm>> GetComponentsForms();
        Task<ServiceResult<ComponentsFormDTO>> CreateComponentForm(CreateComponentFormDTO dto);

        Task<IEnumerable<PermisosDTO>> PermisosRoleListAsignacion(int rolId, int componentsId);
    }
}
