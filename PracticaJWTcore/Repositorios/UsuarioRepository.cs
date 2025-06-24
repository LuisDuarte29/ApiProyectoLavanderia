using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Security;

namespace PracticaJWTcore.Repositorios
{
    public class UsuarioRepository : IUsuariosRepository
    {
        public readonly PracticaJWTcoreContext _context;
        string _conection;
        public UsuarioRepository(PracticaJWTcoreContext context, IConfiguration configuration)
        {
            _context = context;
            _conection = configuration.GetConnectionString("DefaultConnection");


        }
        public async Task<bool> CreateUsuarios(CreateUsuariosDTO usuarioCreate)
        {
            bool result = false;
            using (var conecction = new SqlConnection(_conection))
            {
                using (var command = new SqlCommand("proc_InsertUsuario", conecction))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Correo", usuarioCreate.correo);
                    command.Parameters.AddWithValue("@CustomerId", usuarioCreate.CustomerID);
                    command.Parameters.AddWithValue("@RoleId", usuarioCreate.RoleId);
                    await conecction.OpenAsync();
                    int filasAfectadas = await command.ExecuteNonQueryAsync();
                    if (filasAfectadas > 0)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                        throw new Exception("Error al crear el usuario");

                    }
                    await _context.SaveChangesAsync();
                    await conecction.CloseAsync();
                }

                return result;

            }

        }


        public async Task<List<RoleModal>> GetAllRoles()
        {
            return await _context.Roles.Select(x => new RoleModal
            {
                RoleId = x.RoleId,
                RoleName = x.RoleName
            }).ToListAsync();
        }

        public async Task<List<UsuarioDTO>> GetAllUsuarios()
        {
            return await _context.Usuarios.Select(x => new UsuarioDTO
            {
                IdUsuario = x.IdUsuario,
                Correo = x.correo,
                Customer = _context.Customer.Where(c => c.Id == x.CustomerID).Select(p => p.FirstName).FirstOrDefault(),
                Role = _context.Roles.Where(rol => rol.RoleId == x.RoleId).Select(c => c.RoleName).FirstOrDefault(),
            }).ToListAsync();
        }

        public async Task<List<RolesDTO>> GetRolesList()
        {
            return await _context.Roles.Select(x => new RolesDTO
            {
                RoleId = x.RoleId,
                RoleName = x.RoleName
            }).ToListAsync();
        }
        public async Task<List<PermisosDTO>> PermisosRoleList(int roleId)
        {
            var permisos = await _context.RolesPermisos.Where(rp => rp.RoleId == roleId)
                .Select(rp => new PermisosDTO
                {
                    PermisoId = rp.PermisoId
                }).ToListAsync();

            return permisos;
        }
        public async Task<List<PermisosModal>> GetPermisosList()
        {
            return await _context.Permisos.Select(x => new PermisosModal
            {
                PermisoId = x.PermisoId,
                PermisoName = x.PermisoNombre
            }).ToListAsync();
        }

        public async Task<bool> PermisosRoleCreate(RolesPermisoDTO r)
        {
            var rolesPermisos = await _context.RolesPermisos.Where(x => x.RoleId == r.RoleId).Select(a => a.PermisoId).ToListAsync();

            int[] permisosIdAdd = r.PermisosId.Except(rolesPermisos).ToArray();
            var permisosIdRemove = rolesPermisos.Except(r.PermisosId);

            if (permisosIdAdd.Length > 0)
            {
                foreach (var permisoId in permisosIdAdd)
                {
                    var newRolePermiso = new RolesPermisos
                    {
                        RoleId = r.RoleId,
                        PermisoId = permisoId
                    };
                    _context.RolesPermisos.Add(newRolePermiso);
                   _context.SaveChanges();
                }

            }
            if (permisosIdRemove.Any())
            {
                foreach (var permisoId in permisosIdRemove)
                {
                    var rolePermiso = await _context.RolesPermisos.FirstOrDefaultAsync(x => x.RoleId == r.RoleId && x.PermisoId == permisoId);
                    if (rolePermiso != null)
                    {
                        _context.RolesPermisos.Remove(rolePermiso);
                        await _context.SaveChangesAsync();
                    }
                }
            }


            return true;

        }

        
    }
}
