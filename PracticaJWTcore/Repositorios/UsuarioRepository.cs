using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Identity;
using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;
using System.Linq.Expressions;
using System.Security;

namespace PracticaJWTcore.Repositorios
{
    // Repository de usuarios: crea claves con PasswordHasher y usa EF Core para roles/permisos.
    public class UsuarioRepository : IUsuariosRepository
    {
        public readonly PracticaJWTcoreContext _context;
        private readonly PasswordHasher<Usuarios> _passwordHasher = new();
        public UsuarioRepository(PracticaJWTcoreContext context, IConfiguration configuration)
        {
            _context = context;
        }
        public async Task<bool> CreateUsuarios(CreateUsuariosDTO usuarioCreate)
        {
            try
            {
                var usuario = new Usuarios
                {
                    correo = usuarioCreate.correo.Trim(),
                    CustomerID = usuarioCreate.CustomerID,
                    RoleId = usuarioCreate.RoleId,
                    Customer = null,
                    Role = null
                };

                var claveInicial = usuarioCreate.clave!.Trim();
                // El hash queda en SQL Server como nvarchar(256); la clave original no se persiste.
                usuario.clave = _passwordHasher.HashPassword(usuario, claveInicial);

                await _context.Usuarios.AddAsync(usuario);
                int rowsAffected = await _context.SaveChangesAsync();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el usuario: {ex.Message}");
                return false;
            }

        }

        public async Task<RolesDTO> CreateRole(CreateRoleDTO dto)
        {
            var role = new Roles
            {
                RoleName = dto.RoleName!.Trim()
            };

            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            return new RolesDTO
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName
            };
        }

        public async Task<RolesDTO?> UpdateRole(int roleId, UpdateRoleDTO dto)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == roleId);
            if (role == null)
                return null;

            role.RoleName = dto.RoleName!.Trim();
            await _context.SaveChangesAsync();

            return new RolesDTO
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName
            };
        }

        public async Task<bool> RoleExists(int roleId)
        {
            return roleId > 0 && await _context.Roles.AnyAsync(r => r.RoleId == roleId);
        }

        public async Task<bool> RoleNameExists(string roleName, int? excludeRoleId = null)
        {
            var normalizedName = roleName.Trim();
            return await _context.Roles.AnyAsync(r =>
                r.RoleName == normalizedName && (!excludeRoleId.HasValue || r.RoleId != excludeRoleId.Value));
        }

        public Task<bool> UsuarioCorreoExists(string correo, int? excludeUsuarioId = null)
        {
            var normalizedCorreo = correo.Trim();
            return _context.Usuarios.AnyAsync(u =>
                u.correo == normalizedCorreo && (!excludeUsuarioId.HasValue || u.IdUsuario != excludeUsuarioId.Value));
        }


        public async Task<IEnumerable<RoleModal>> GetAllRoles()
        {
            return await _context.Roles.Select(x => new RoleModal
            {
                RoleId = x.RoleId,
                RoleName = x.RoleName
            }).ToListAsync();
        }

        public async Task<IEnumerable<UsuarioDTO>> GetAllUsuarios()
        {
            return await _context.Usuarios.AsNoTracking().Include(c => c.Customer)
                .Include(r => r.Role)
                .Select(x => new UsuarioDTO
                {
                    IdUsuario = x.IdUsuario,
                    Correo = x.correo,
                    RoleId = x.RoleId,
                    Roles = x.Role != null ? x.Role.RoleName : string.Empty,
                    CustomerID = x.CustomerID,
                    Customer = x.Customer != null ? x.Customer.FirstName : string.Empty
                }).ToListAsync();
        }



        public async Task<IEnumerable<RolesDTO>> GetRolesList()
        {
            return await _context.Roles.Select(x => new RolesDTO
            {
                RoleId = x.RoleId,
                RoleName = x.RoleName
            }).ToListAsync();
        }
        public async Task<IEnumerable<PermisosDTO>> PermisosRoleList(int roleId, string ComponentsString)
        {   
            var ComponentsId = await _context.ComponentsForm.Where(c => c.ComponentsName == ComponentsString).Select(c => c.ComponentsId).FirstOrDefaultAsync();

            var permisos = await _context.RolesPermisos.Where(rp => rp.RoleId == roleId  && rp.ComponentsId == ComponentsId)
                .Select(rp => rp.PermisoId)
                .Distinct()
                .Select(permisoId => new PermisosDTO
                {
                    PermisoId = permisoId
                }).ToListAsync();

            return permisos;
        }

 
        public async Task<IEnumerable<PermisosDTO>> PermisosRoleListAsignacion(int roleId, int ComponenteId)
        {
         

            var permisos = await _context.RolesPermisos.Where(rp => rp.RoleId == roleId && rp.ComponentsId == ComponenteId)
                .Select(rp => rp.PermisoId)
                .Distinct()
                .Select(permisoId => new PermisosDTO
                {
                    PermisoId = permisoId
                }).ToListAsync();

            return permisos;
        }
        public async Task<IEnumerable<PermisosModal>> GetPermisosList()
        {
            return await _context.Permisos.Select(x => new PermisosModal
            {
                PermisoId = x.PermisoId,
                PermisoName = x.PermisoNombre
            }).ToListAsync();
        }

        public async Task<bool> PermisosRoleCreate(RolesPermisoDTO r)
        {
            var permisosSolicitados = r.PermisosId.Distinct().ToHashSet();
            var existentes = await _context.RolesPermisos
                .Where(x => x.RoleId == r.RoleId && x.ComponentsId == r.ComponentsFormId)
                .ToListAsync();

            var permisosExistentes = existentes.Select(x => x.PermisoId).Distinct().ToHashSet();
            var permisosParaAgregar = permisosSolicitados.Except(permisosExistentes).ToList();
            var permisosParaQuitar = existentes
                .Where(x => !permisosSolicitados.Contains(x.PermisoId))
                .ToList();

            var duplicadosParaQuitar = existentes
                .Where(x => permisosSolicitados.Contains(x.PermisoId))
                .GroupBy(x => x.PermisoId)
                .SelectMany(g => g.OrderBy(x => x.RolePermisoId).Skip(1))
                .ToList();

            if (permisosParaQuitar.Any())
                _context.RolesPermisos.RemoveRange(permisosParaQuitar);

            if (duplicadosParaQuitar.Any())
                _context.RolesPermisos.RemoveRange(duplicadosParaQuitar);

            foreach (var permisoId in permisosParaAgregar)
            {
                _context.RolesPermisos.Add(new RolesPermisos
                {
                    RoleId = r.RoleId,
                    PermisoId = permisoId,
                    ComponentsId = r.ComponentsFormId
                });
            }

            await _context.SaveChangesAsync();

            return true;

        }
       public async Task<IEnumerable<ComponentsForm>> GetComponentsForms()
        {
            return await _context.ComponentsForm.Select(x => new ComponentsForm
            {
                ComponentsId= x.ComponentsId,
                ComponentsName = x.ComponentsName
        
            }).ToListAsync();
        }

        public async Task<ComponentsFormDTO> CreateComponentForm(CreateComponentFormDTO dto)
        {
            var component = new ComponentsForm
            {
                ComponentsName = dto.ComponentsName!.Trim()
            };

            await _context.ComponentsForm.AddAsync(component);
            await _context.SaveChangesAsync();

            return new ComponentsFormDTO
            {
                ComponentsId = component.ComponentsId,
                ComponentsName = component.ComponentsName
            };
        }

        public async Task<bool> ComponentExists(int componentsFormId)
        {
            return componentsFormId > 0 && await _context.ComponentsForm.AnyAsync(c => c.ComponentsId == componentsFormId);
        }

        public async Task<bool> ComponentNameExists(string componentsName, int? excludeComponentsId = null)
        {
            var normalizedName = componentsName.Trim();
            return await _context.ComponentsForm.AnyAsync(c =>
                c.ComponentsName == normalizedName && (!excludeComponentsId.HasValue || c.ComponentsId != excludeComponentsId.Value));
        }

        public async Task<bool> PermisosExist(IEnumerable<int> permisosId)
        {
            var ids = permisosId.Distinct().ToList();
            if (ids.Any(id => id <= 0))
                return false;

            if (!ids.Any())
                return true;

            var cantidad = await _context.Permisos.CountAsync(p => ids.Contains(p.PermisoId));
            return cantidad == ids.Count;
        }
        

    }
}
