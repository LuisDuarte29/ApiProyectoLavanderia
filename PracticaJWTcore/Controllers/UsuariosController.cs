using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Repositorios;
using PracticaJWTcore.Models;
using PracticaJWTcore.Dtos;

namespace PracticaJWTcore.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IUsuariosRepository _usuariosRepository;
        public UsuariosController(IUsuariosRepository usuariosRepository)
        {
            _usuariosRepository = usuariosRepository;
        }

        [HttpPost("api/Usuarios/CreateUsuario")]
        public async Task<IActionResult> CreateUsuario(CreateUsuariosDTO createUsuario)
        {
            bool Usuario = await _usuariosRepository.CreateUsuarios(createUsuario);
            return Usuario ? Ok() : NotFound();
        }
        [HttpGet("api/Usuarios/GetRoles")]
        public async Task<IActionResult> GetAllRoles()
         
       {
            var roles = await _usuariosRepository.GetAllRoles();
            return new OkObjectResult(roles);
        }
        [HttpGet("api/Usuarios/GetUsuarios")]
        public async Task<IActionResult> GetAllUsuarios()
        {
            var usuarios = await _usuariosRepository.GetAllUsuarios();
            return new OkObjectResult(usuarios);
        }
    }
}
