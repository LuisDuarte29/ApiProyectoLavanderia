using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public interface IUsuariosRepository
    {
        Task<IEnumerable<UsuarioDTO>> GetAllUsuarios();
        Task<bool> CreateUsuarios(CreateUsuariosDTO usuarioCreate);
        Task<RolesDTO> CreateRole(CreateRoleDTO dto);
        Task<RolesDTO?> UpdateRole(int roleId, UpdateRoleDTO dto);
        Task<bool> RoleExists(int roleId);
        Task<bool> RoleNameExists(string roleName, int? excludeRoleId = null);
        Task<bool> UsuarioCorreoExists(string correo, int? excludeUsuarioId = null);
        Task<IEnumerable<RoleModal>> GetAllRoles();
        Task<IEnumerable<RolesDTO>> GetRolesList();
        Task<IEnumerable<PermisosDTO>> PermisosRoleList(int roleId,string ComponentsId);
        Task<IEnumerable<PermisosModal>> GetPermisosList();
        Task<bool> PermisosRoleCreate(RolesPermisoDTO rolesPermisos);
        Task<IEnumerable<ComponentsForm>> GetComponentsForms();
        Task<ComponentsFormDTO> CreateComponentForm(CreateComponentFormDTO dto);
        Task<bool> ComponentExists(int componentsFormId);
        Task<bool> ComponentNameExists(string componentsName, int? excludeComponentsId = null);
        Task<bool> PermisosExist(IEnumerable<int> permisosId);

        Task<IEnumerable<PermisosDTO>> PermisosRoleListAsignacion(int roleId, int componentsFormId);

    }
}
