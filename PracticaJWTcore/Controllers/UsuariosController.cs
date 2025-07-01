using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Repositorios;
using PracticaJWTcore.Models;
using PracticaJWTcore.Dtos;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UsuariosController : Controller
    {
        private readonly IUsuariosRepository _usuariosRepository;
        public UsuariosController(IUsuariosRepository usuariosRepository)
        {
            _usuariosRepository = usuariosRepository;
        }

        [HttpPost("CreateUsuario")]
        public async Task<IActionResult> CreateUsuario([FromBody] CreateUsuariosDTO dto)
        {
            try
            {
                bool creado = await _usuariosRepository.CreateUsuarios(dto);
                if (!creado)
                    return BadRequest("No se pudo crear el usuario. Verifica datos e intenta de nuevo.");

                return Ok(new { mensaje = "Usuario creado exitosamente" });
            }
            catch (Exception ex)
            {
                // Opcional: loguea aquí ex.Message
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("GetRoles")]
        public async Task<IActionResult> GetAllRoles()

        {
            var roles = await _usuariosRepository.GetAllRoles();
            return new OkObjectResult(roles);
        }
        [HttpGet("GetUsuarios")]
        public async Task<IActionResult> GetAllUsuarios()
        {
            var usuarios = await _usuariosRepository.GetAllUsuarios();
            return new OkObjectResult(usuarios);
        }
        [HttpGet("GetRoleList")]
        public async Task<IActionResult> GetRoleList()
        {
            var rolesList = await _usuariosRepository.GetRolesList();
            return new OkObjectResult(rolesList);
        }
        [HttpGet("GetListPermisos/{roleId}/{componentsFormSelect}")]
        public async Task<IActionResult> GetListPermisos(int roleId,string componentsFormSelect)
       
        {
            var permisosList = await _usuariosRepository.PermisosRoleList(roleId , componentsFormSelect);
            return new OkObjectResult(permisosList);
        }

        [HttpGet("GetListPermisosAsignacion/{roleId}/{componentsFormSelect}")]
        public async Task<IActionResult> GetListPermisosAsignacion(int roleId, int componentsFormSelect)

        {
            var permisosList = await _usuariosRepository.PermisosRoleListAsignacion(roleId, componentsFormSelect);
            return new OkObjectResult(permisosList);
        }

        [HttpGet("GetPermisosList")]
        public async Task<IActionResult> GetPermisosList()
        {
            var permisosList = await _usuariosRepository.GetPermisosList();
            return new OkObjectResult(permisosList);
        }
        [HttpPut("CreatePermisosRole")]
        public async Task<IActionResult> CreatePermisosRole([FromBody] RolesPermisoDTO rolesPermisos)
        {
            bool result = await _usuariosRepository.PermisosRoleCreate(rolesPermisos);
            return result ? Ok() : NotFound();
        }
        [HttpGet("GetComponentsForms")]
        public async Task<IActionResult> GetComponentsForms()
        {
            var componentsForms = await _usuariosRepository.GetComponentsForms();
            return new OkObjectResult(componentsForms);
        }
    }
}
