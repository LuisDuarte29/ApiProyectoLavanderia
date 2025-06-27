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
        public async Task<IActionResult> CreateUsuario([FromBody] CreateUsuariosDTO createUsuario)
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
        [HttpGet("api/Usuarios/GetRoleList")]
        public async Task<IActionResult> GetRoleList()
        {
            var rolesList = await _usuariosRepository.GetRolesList();
            return new OkObjectResult(rolesList);
        }
        [HttpGet("api/Usuarios/GetListPermisos/{roleId}/{componentsFormSelect}")]
        public async Task<IActionResult> GetListPermisos(int roleId,string componentsFormSelect)
       {
            var permisosList = await _usuariosRepository.PermisosRoleList(roleId , componentsFormSelect);
            return new OkObjectResult(permisosList);
        }
        [HttpGet("api/Usuarios/GetPermisosList")]
        public async Task<IActionResult> GetPermisosList()
        {
            var permisosList = await _usuariosRepository.GetPermisosList();
            return new OkObjectResult(permisosList);
        }
        [HttpPut("api/Usuarios/CreatePermisosRole")]
        public async Task<IActionResult> CreatePermisosRole([FromBody] RolesPermisoDTO rolesPermisos)
        {
            bool result = await _usuariosRepository.PermisosRoleCreate(rolesPermisos);
            return result ? Ok() : NotFound();
        }
        [HttpGet("api/Usuarios/GetComponentsForms")]
        public async Task<IActionResult> GetComponentsForms()
        {
            var componentsForms = await _usuariosRepository.GetComponentsForms();
            return new OkObjectResult(componentsForms);
        }
    }
}
