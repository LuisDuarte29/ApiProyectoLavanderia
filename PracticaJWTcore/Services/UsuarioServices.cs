using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;

namespace PracticaJWTcore.Services
{
    // Service de usuarios, roles y permisos; delega persistencia al repository y conserva la capa de negocio.
    public class UsuarioServices : IUsuarioServices
    {
        private readonly IUsuariosRepository _context;
        public UsuarioServices(IUsuariosRepository context)
        {
            _context = context;
        }
        public async Task<bool> CreateUsuarios(CreateUsuariosDTO usuarioCreate)
        {
            if (usuarioCreate == null)
                return false;

            if (string.IsNullOrWhiteSpace(usuarioCreate.correo))
                return false;

            if (string.IsNullOrWhiteSpace(usuarioCreate.clave))
                return false;

            if (!await _context.RoleExists(usuarioCreate.RoleId))
                return false;

            if (await _context.UsuarioCorreoExists(usuarioCreate.correo))
                return false;

            return await _context.CreateUsuarios(usuarioCreate);
        }

        public async Task<ServiceResult<RolesDTO>> CreateRole(CreateRoleDTO dto)
        {
            var roleName = dto?.RoleName?.Trim();
            if (string.IsNullOrWhiteSpace(roleName))
                return ServiceResult<RolesDTO>.Fail("RoleName es requerido", "role_name_required");

            if (await _context.RoleNameExists(roleName))
                return ServiceResult<RolesDTO>.Fail("Ya existe un rol con ese nombre", "role_duplicate");

            var role = await _context.CreateRole(new CreateRoleDTO { RoleName = roleName });
            return ServiceResult<RolesDTO>.Ok(role);
        }

        public async Task<ServiceResult<RolesDTO>> UpdateRole(int roleId, UpdateRoleDTO dto)
        {
            var roleName = dto?.RoleName?.Trim();
            if (string.IsNullOrWhiteSpace(roleName))
                return ServiceResult<RolesDTO>.Fail("RoleName es requerido", "role_name_required");

            if (!await _context.RoleExists(roleId))
                return ServiceResult<RolesDTO>.Fail("Rol no encontrado", "not_found");

            if (await _context.RoleNameExists(roleName, roleId))
                return ServiceResult<RolesDTO>.Fail("Ya existe un rol con ese nombre", "role_duplicate");

            var role = await _context.UpdateRole(roleId, new UpdateRoleDTO { RoleName = roleName });
            return role == null
                ? ServiceResult<RolesDTO>.Fail("Rol no encontrado", "not_found")
                : ServiceResult<RolesDTO>.Ok(role);
        }

        public async Task<IEnumerable<RoleModal>> GetAllRoles()
        {
            return await _context.GetAllRoles();
        }

        public Task<IEnumerable<UsuarioDTO>> GetAllUsuarios()
        {
            return _context.GetAllUsuarios();
        }

        public async Task<IEnumerable<PermisosModal>> GetPermisosList()
        {
            return await _context.GetPermisosList();
        }

        public async Task<IEnumerable<RolesDTO>> GetRolesList()
        {
            return await _context.GetRolesList();
        }

        public async Task<bool> PermisosRoleCreate(RolesPermisoDTO rolesPermisos)
        {
           if (rolesPermisos == null)
               return false;

           if (!await _context.RoleExists(rolesPermisos.RoleId))
               return false;

           if (!await _context.ComponentExists(rolesPermisos.ComponentsFormId))
               return false;

           var permisosId = rolesPermisos.PermisosId?.Distinct().ToList() ?? new List<int>();
           if (!await _context.PermisosExist(permisosId))
               return false;

           rolesPermisos.PermisosId = permisosId;
           return await _context.PermisosRoleCreate(rolesPermisos);
        }

        public async Task<IEnumerable<PermisosDTO>> PermisosRoleList(int roleId,string componentsId)
        {
            return await _context.PermisosRoleList(roleId,componentsId);
        }

        public async Task<IEnumerable<PermisosDTO>> PermisosRoleListAsignacion(int roleId, int componentsFormId)
        {
            return await _context.PermisosRoleListAsignacion(roleId, componentsFormId);
        }

        public async Task<IEnumerable<ComponentsForm>> GetComponentsForms()
        {
            return await _context.GetComponentsForms();
        }

        public async Task<ServiceResult<ComponentsFormDTO>> CreateComponentForm(CreateComponentFormDTO dto)
        {
            var componentsName = dto?.ComponentsName?.Trim();
            if (string.IsNullOrWhiteSpace(componentsName))
                return ServiceResult<ComponentsFormDTO>.Fail("ComponentsName es requerido", "component_name_required");

            if (await _context.ComponentNameExists(componentsName))
                return ServiceResult<ComponentsFormDTO>.Fail("Ya existe un componente con ese nombre", "component_duplicate");

            var component = await _context.CreateComponentForm(new CreateComponentFormDTO { ComponentsName = componentsName });
            return ServiceResult<ComponentsFormDTO>.Ok(component);
        }

    }
}
