using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Dtos;
using PracticaJWTcore.Services;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Controller de usuarios, roles y permisos: expone endpoints historicos y delega al service.
    public class UsuariosController : Controller
    {
        private readonly IUsuarioServices _usuarioServices;

        public UsuariosController(IUsuarioServices usuarioServices)
        {
            _usuarioServices = usuarioServices;
        }
        [Authorize]
        [HttpPost("CreateUsuario")]
        // Crea usuarios mediante el service; el repository guarda la clave hasheada con EF Core.
        public async Task<IActionResult> CreateUsuario([FromBody] CreateUsuariosDTO dto)
        {
            try
            {
                bool creado = await _usuarioServices.CreateUsuarios(dto);
                if (!creado)
                    return BadRequest("No se pudo crear el usuario. Verifica datos e intenta de nuevo.");

                return Ok(new { mensaje = "Usuario creado exitosamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear usuario: {ex.Message}");
                return StatusCode(500, "Error interno al crear el usuario");
            }
        }
        [Authorize]
        [HttpGet("GetRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _usuarioServices.GetAllRoles();
            return new OkObjectResult(roles);
        }
        [Authorize]
        [HttpGet("GetUsuarios")]
        public async Task<IActionResult> GetAllUsuarios()
        {
            var usuarios = await _usuarioServices.GetAllUsuarios();
            return new OkObjectResult(usuarios);
        }
        [Authorize]
        [HttpGet("GetRoleList")]
        public async Task<IActionResult> GetRoleList()
        {
            var rolesList = await _usuarioServices.GetRolesList();
            return new OkObjectResult(rolesList);
        }
        [Authorize]
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDTO dto)
        {
            var result = await _usuarioServices.CreateRole(dto);
            return result.Success
                ? Ok(result.Value)
                : BadRequest(result.Message);
        }
        [Authorize]
        [HttpPut("UpdateRole/{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleDTO dto)
        {
            var result = await _usuarioServices.UpdateRole(id, dto);
            if (result.Success)
                return Ok(result.Value);

            return result.Code == "not_found"
                ? StatusCode(StatusCodes.Status404NotFound, result.Message)
                : BadRequest(result.Message);
        }
        [Authorize]
        [HttpGet("GetListPermisos/{roleId}/{componentsFormSelect}")]
        public async Task<IActionResult> GetListPermisos(int roleId, string componentsFormSelect)
        {
            var permisosList = await _usuarioServices.PermisosRoleList(roleId, componentsFormSelect);
            return new OkObjectResult(permisosList);
        }
        [Authorize]
        [HttpGet("GetListPermisosAsignacion/{roleId}/{componentsFormSelect}")]
        public async Task<IActionResult> GetListPermisosAsignacion(int roleId, int componentsFormSelect)
        {
            var permisosList = await _usuarioServices.PermisosRoleListAsignacion(roleId, componentsFormSelect);
            return new OkObjectResult(permisosList);
        }
        [Authorize]
        [HttpGet("GetPermisosList")]
        public async Task<IActionResult> GetPermisosList()
        {
            var permisosList = await _usuarioServices.GetPermisosList();
            return new OkObjectResult(permisosList);
        }
        [Authorize]
        [HttpPut("CreatePermisosRole")]
        // Actualiza permisos por rol/componente manteniendo el payload que ya consume la UI.
        public async Task<IActionResult> CreatePermisosRole([FromBody] RolesPermisoDTO rolesPermisos)
        {
            bool result = await _usuarioServices.PermisosRoleCreate(rolesPermisos);
            return result ? Ok() : NotFound();
        }
        [Authorize]
        [HttpGet("GetComponentsForms")]
        public async Task<IActionResult> GetComponentsForms()
        {
            var componentsForms = await _usuarioServices.GetComponentsForms();
            return new OkObjectResult(componentsForms);
        }
        [Authorize]
        [HttpPost("CreateComponentsForm")]
        public async Task<IActionResult> CreateComponentsForm([FromBody] CreateComponentFormDTO dto)
        {
            var result = await _usuarioServices.CreateComponentForm(dto);
            return result.Success
                ? Ok(result.Value)
                : BadRequest(result.Message);
        }
    }
}
