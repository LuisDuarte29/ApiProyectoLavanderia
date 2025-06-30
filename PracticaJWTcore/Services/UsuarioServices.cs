using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;

namespace PracticaJWTcore.Services
{
    public class UsuarioServices : IUsuarioServices
    {
        private readonly IUsuariosRepository _context;
        public UsuarioServices(IUsuariosRepository context)
        {
            _context = context;
        }
        public async Task<bool> CreateUsuarios(CreateUsuariosDTO usuarioCreate)
        {
            return await _context.CreateUsuarios(usuarioCreate);
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
           return await _context.PermisosRoleCreate(rolesPermisos);
        }

        public async Task<IEnumerable<PermisosDTO>> PermisosRoleList(int roleId,string componentsId)
        {
            return await _context.PermisosRoleList(roleId,componentsId);
        }

       public async Task<IEnumerable<ComponentsForm>> GetComponentsForms()
        {
            return await _context.GetComponentsForms();
        }


    }
}
