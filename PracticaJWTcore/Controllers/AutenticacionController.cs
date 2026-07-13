using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;
using PracticaJWTcore.Services;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Controller de autenticacion: mantiene el contrato historico del login y delega validacion/JWT al service.
    public class AutenticacionController : Controller
    {
        public readonly IAutenticacionServices _autenticacionServices;

        public AutenticacionController(IAutenticacionServices autenticacionServices)
        {
            _autenticacionServices = autenticacionServices;
        }

        [HttpPost]
        // POST /api/Autenticacion recibe correo/clave y devuelve el wrapper tokenRol que usa el frontend.
        public async Task<IActionResult> Login([FromBody] UsuarioLogin loginModel)
        {
            TokenRolDTO response = await _autenticacionServices.Login(loginModel);
            return response != null
                ? StatusCode(StatusCodes.Status200OK, new { tokenRol = response })
                : StatusCode(StatusCodes.Status204NoContent, new { string.Empty });
        }
        [Authorize]
        [HttpPost("CambioClave")]
        // CambioClave conserva una respuesta simple para el flujo historico de cambio de password.
        public async Task<IActionResult> CambioClave([FromBody] CambioClave cambio)
        {
            var response = await _autenticacionServices.CambioClave(cambio);

            return response > 0
                ? Ok()
                : base.StatusCode(StatusCodes.Status400BadRequest, new { message = "Error debe de proporcionar una clave" });
        }
    }
}
