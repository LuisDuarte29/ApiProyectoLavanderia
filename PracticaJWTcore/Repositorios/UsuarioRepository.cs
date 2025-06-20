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
        public UsuarioRepository(PracticaJWTcoreContext context,IConfiguration configuration) 
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
                   int filasAfectadas= await command.ExecuteNonQueryAsync();
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
                IdUsuario=x.IdUsuario,
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
    }
}
