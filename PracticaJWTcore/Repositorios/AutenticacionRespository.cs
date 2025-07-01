
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PracticaJWTcore.Dtos;
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
        public AutenticacionRespository(PracticaJWTcoreContext context, IConfiguration configuration)
        {
            _context = context;
            this.secretKey = configuration.GetSection("JWT").GetSection("Key").ToString();
            this.conection = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<TokenRolDTO> GenerateToken(UsuarioLogin usuario, int result,int rolId,string rolName)
        {

            byte[] token;
            string TokenCreado = "";
            if (result == 1)
            {
                token = Encoding.ASCII.GetBytes(secretKey);
                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.Role, rolName));
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
                TokenRolDTO tokenRol = new TokenRolDTO
                {
                    Token = tokenCread,
                    RolId = rolId
                };
                return tokenRol;
            }
            else
            {
                 TokenRolDTO tokenVacio= new TokenRolDTO
                {
                    Token = "",
                    RolId = 0
                };
                return tokenVacio;
             }
        }



        public async Task<TokenRolDTO> Login(UsuarioLogin usuario)
        {

          string rolName = string.Empty;
            int roleId = 0;
            int result = 0;
            using (var conecction = new SqlConnection(conection))
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

                    if (result > 0)
                    {
                       roleId=await _context.Usuarios
                            .Where(u => u.correo == usuario.correo)
                            .Select(u => u.RoleId)
                            .FirstOrDefaultAsync();
                        rolName = await _context.Roles.Where(r => r.RoleId == roleId)
                            .Select(r => r.RoleName)
                            .FirstOrDefaultAsync();
                    }
                    await conecction.CloseAsync();
                }
               
                return await GenerateToken(usuario,result,roleId, rolName);

            }
            
        }
        public Task<int> CambioClave(CambioClave cambios)
        {
            int filasAfectadas = 0;
            using (var conecction = new SqlConnection(conection))
            {

                using (var command=new SqlCommand("proc_ComparePass", conecction))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@correo", cambios.correo);
                    command.Parameters.AddWithValue("@Clave", cambios.claveActual);
                    SqlParameter ouputParameter = command.Parameters.Add("@Compare", SqlDbType.Int);
                    ouputParameter.Direction = ParameterDirection.Output;
                    conecction.Open();

                    command.ExecuteNonQuery();

                   int result = (int)ouputParameter.Value;
                    conecction.Close();
                    if (result != 0)
                    {
                        using (var commandCambio = new SqlCommand("proc_CambioClave", conecction))
                        {
                            commandCambio.CommandType = CommandType.StoredProcedure;
                            commandCambio.Parameters.AddWithValue("@correo", cambios.correo);
                            commandCambio.Parameters.AddWithValue("@NuevaClave", cambios.nuevaClave);

                            conecction.Open();
                             filasAfectadas = commandCambio.ExecuteNonQuery();
                            conecction.Close();
                       

                        }
                    }
                }


             
            }
            if (filasAfectadas > 0)
            {
                return Task.FromResult(1); // Cambio exitoso
            }
            else
            {
                return Task.FromResult(0); // No se realizó ningún cambio 

            }

        }

     
    }
}
