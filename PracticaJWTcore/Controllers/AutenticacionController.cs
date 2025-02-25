
using ApiSwagger.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("Autenticacion")]
    public class AutenticacionController : Controller
    {
        private readonly string secretKey;
        private readonly string conection;
        public AutenticacionController(IConfiguration configuration)
        {
            this.secretKey = configuration.GetSection("JWT").GetSection("key").ToString();
            this.conection = configuration.GetConnectionString("DefaultConnection");
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Usuario request)
        {
            int result = 0;
            using (var conecction = new SqlConnection(conection))
            {
                using (var command=new SqlCommand("proc_ComparePass", conecction))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Correo", request.correo);
                    command.Parameters.AddWithValue("@Clave", request.clave);
                    SqlParameter ouputParameter = command.Parameters.Add("@Compare", SqlDbType.Int);
                    ouputParameter.Direction = ParameterDirection.Output;
                    await conecction.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    result = (int)ouputParameter.Value;
                    await conecction.CloseAsync();
                }

            }
                if (result==1)
                {
                    var token = Encoding.ASCII.GetBytes(secretKey);
                    var claims = new ClaimsIdentity();
                    claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.correo));
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = claims,
                        Expires = DateTime.UtcNow.AddHours(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(token), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenCread = tokenHandler.WriteToken(tokenConfig);
                    return StatusCode(StatusCodes.Status200OK, new { token = tokenCread });
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });
                }
        
        }
    }
}
