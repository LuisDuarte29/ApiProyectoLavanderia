
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PracticaJWTcore.Models;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PracticaJWTcore.Repositorios
{
    public class AutenticacionRespository : IAutenticacionRepository
    {
        private readonly PracticaJWTcoreContext _context;
        private readonly string secretKey;
        private readonly string conection;
        public AutenticacionRespository(PracticaJWTcoreContext context,IConfiguration configuration)
        {
            _context = context;
            this.secretKey = configuration.GetSection("JWT").GetSection("Key").ToString();
            this.conection = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<string> GenerateToken(UsuarioLogin usuario, List<string> roles, int result)
        {
          
            byte[] token;
            string TokenCreado = "";
            if (result == 1)
            {
                token = Encoding.ASCII.GetBytes(secretKey);
                var claims = new ClaimsIdentity();
                foreach (var rol in roles)
                {
                    claims.AddClaim(new Claim(ClaimTypes.Role, rol));
                }
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.correo));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(token), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
                var tokenCread = tokenHandler.WriteToken(tokenConfig);
                return TokenCreado=tokenCread;
            }
            else
            {
                return TokenCreado = "";
            }
        }

   

        public async Task<string> Login(UsuarioLogin usuario)
        {

            var usuarioRoles = await _context.Usuarios.Include(u => u.UsuariosRoles).ThenInclude(ur => ur.Rol)
                .ThenInclude(o => o.RolesPermisos).ThenInclude(p => p.Permisos).FirstOrDefaultAsync(u => u.Correo == usuario.correo);

            int result = 0;
            using (var conecction = new SqlConnection())
            {
                using (var command = new SqlCommand("proc_ComparePass", conecction))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Correo", usuario.correo);
                    command.Parameters.AddWithValue("@Clave", usuario.clave);
                    SqlParameter ouputParameter = command.Parameters.Add("@Compare", SqlDbType.Int);
                    ouputParameter.Direction = ParameterDirection.Output;
                    await conecction.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    result = (int)ouputParameter.Value;
                    await conecction.CloseAsync();
                }
            var Roles= usuarioRoles.UsuariosRoles.Select(x=>x.Rol.RoleName).ToList();
                return await GenerateToken(usuario, Roles, result);

            }
          
        }

      
    }
}
