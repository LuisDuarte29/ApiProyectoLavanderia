using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
            try
            {
                await using var conn = new SqlConnection(_conection);
                await using var cmd = new SqlCommand("proc_InsertUsuario", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Parámetros
                cmd.Parameters.Add(new SqlParameter("@Correo", SqlDbType.VarChar, 200) { Value = usuarioCreate.correo });
                cmd.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.BigInt) { Value = usuarioCreate.CustomerID });
                cmd.Parameters.Add(new SqlParameter("@RoleId", SqlDbType.Int) { Value = usuarioCreate.RoleId });

                await conn.OpenAsync();
                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                return rowsAffected > 0;   // true si insertó al menos una fila
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el usuario por SP: {ex.Message}");
                return false;
            }

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
        

            return await _context.Usuarios.AsTracking().Include(c => c.Customer)
                .Include(r => r.Role)
                .Select(x => new UsuarioDTO
                {
                    IdUsuario = x.IdUsuario,
                    Correo = x.correo,
                    Customer = x.Customer.FirstName,
                    Roles = x.Role.RoleName
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
                .Select(rp => new PermisosDTO
                {
                    PermisoId = rp.PermisoId
                }).ToListAsync();

            return permisos;
        }

 
        public async Task<IEnumerable<PermisosDTO>> PermisosRoleListAsignacion(int roleId, int ComponenteId)
        {
         

            var permisos = await _context.RolesPermisos.Where(rp => rp.RoleId == roleId && rp.ComponentsId == ComponenteId)
                .Select(rp => new PermisosDTO
                {
                    PermisoId = rp.PermisoId
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
            var rolesPermisos = await _context.RolesPermisos.AsNoTracking().Where(x => x.RoleId == r.RoleId && x.ComponentsId==r.ComponentsFormId).Select(a => a.PermisoId).ToListAsync();

            int[] permisosIdAdd = r.PermisosId.Except(rolesPermisos).ToArray();
            var permisosIdRemove = rolesPermisos.Except(r.PermisosId);

            if (permisosIdAdd.Length > 0)
            {
                foreach (var permisoId in permisosIdAdd)
                {
                    var newRolePermiso = new RolesPermisos
                    {
                        RoleId = r.RoleId,
                        PermisoId = permisoId,
                        ComponentsId = r.ComponentsFormId

                    };
                     _context.RolesPermisos.Add(newRolePermiso);
              
                }
                await _context.SaveChangesAsync();

            }
            if (permisosIdRemove.Any())
            {
                foreach (var permisoId in permisosIdRemove)
                {
                    var rolePermiso = await _context.RolesPermisos.AsNoTracking().FirstOrDefaultAsync(x => x.RoleId == r.RoleId && x.PermisoId == permisoId);
                    if (rolePermiso != null)
                    {
                        _context.RolesPermisos.Remove(rolePermiso);
               
                    }
                }
                await _context.SaveChangesAsync();
            }


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
        

    }
}
