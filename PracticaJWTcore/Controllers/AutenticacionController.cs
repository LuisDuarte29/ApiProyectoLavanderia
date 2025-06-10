
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacionController : Controller
    {
        public readonly IAutenticacionRepository _autenticacionRepository;
        public AutenticacionController(IAutenticacionRepository autenticacionRepository)
        {
            _autenticacionRepository = autenticacionRepository;
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UsuarioLogin loginModel)
        {
            var response= await _autenticacionRepository.Login(loginModel);
            return response!=null ? StatusCode(StatusCodes.Status200OK, new { token = response }) : StatusCode(StatusCodes.Status204NoContent, new { token = "" });
      
        
        }
        [HttpPost("CambioClave")]
        public async Task<IActionResult> CambioClave([FromBody] CambioClave cambio)
        {
            var response = await _autenticacionRepository.CambioClave(cambio);

            return response > 0 ? Ok(): base.StatusCode(StatusCodes.Status400BadRequest, new { message = "Error al cambiar la clave" });
        }
    }
}
