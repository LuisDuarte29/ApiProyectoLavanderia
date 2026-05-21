using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Controllers;
using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;
using PracticaJWTcore.Services;

namespace PracticaJWTCoreTest;

public class ControllerServiceWiringTests
{
    // Estas pruebas verifican contratos de controllers que el frontend puede depender.
    [Fact]
    public async Task AutenticacionController_Login_mantiene_respuesta_tokenRol()
    {
        var controller = new AutenticacionController(new FakeAutenticacionServices());

        var response = await controller.Login(new UsuarioLogin { correo = "a@b.com", clave = "123" });

        var objectResult = Assert.IsType<ObjectResult>(response);
        Assert.Equal(200, objectResult.StatusCode);
        Assert.NotNull(objectResult.Value?.GetType().GetProperty("tokenRol"));
    }

    [Fact]
    public async Task UsuariosController_CreateUsuario_no_expone_mensaje_interno()
    {
        var controller = new UsuariosController(new ThrowingUsuarioServices());

        var response = await controller.CreateUsuario(new CreateUsuariosDTO());

        var objectResult = Assert.IsType<ObjectResult>(response);
        Assert.Equal(500, objectResult.StatusCode);
        Assert.Equal("Error interno al crear el usuario", objectResult.Value);
    }

    [Fact]
    public async Task UsuariosController_CreateRole_delega_en_service_y_devuelve_ok()
    {
        var controller = new UsuariosController(new FakeUsuarioServices());

        var response = await controller.CreateRole(new CreateRoleDTO { RoleName = "Vendedor" });

        var okResult = Assert.IsType<OkObjectResult>(response);
        var role = Assert.IsType<RolesDTO>(okResult.Value);
        Assert.Equal(10, role.RoleId);
        Assert.Equal("Vendedor", role.RoleName);
    }

    [Fact]
    public async Task UsuariosController_UpdateRole_devuelve_not_found_si_no_existe()
    {
        var controller = new UsuariosController(new FakeUsuarioServices());

        var response = await controller.UpdateRole(404, new UpdateRoleDTO { RoleName = "No existe" });

        var objectResult = Assert.IsType<ObjectResult>(response);
        Assert.Equal(404, objectResult.StatusCode);
    }

    [Fact]
    public async Task UsuariosController_CreateComponentsForm_delega_en_service_y_devuelve_ok()
    {
        var controller = new UsuariosController(new FakeUsuarioServices());

        var response = await controller.CreateComponentsForm(new CreateComponentFormDTO { ComponentsName = "Ventas" });

        var okResult = Assert.IsType<OkObjectResult>(response);
        var component = Assert.IsType<ComponentsFormDTO>(okResult.Value);
        Assert.Equal(12, component.ComponentsId);
        Assert.Equal("Ventas", component.ComponentsName);
    }

    private sealed class FakeAutenticacionServices : IAutenticacionServices
    {
        public Task<int> CambioClave(CambioClave cambio) => Task.FromResult(1);

        public Task<TokenRolDTO> Login(UsuarioLogin usuario)
        {
            return Task.FromResult(new TokenRolDTO
            {
                Token = "token",
                RolId = 1
            });
        }
    }

    private sealed class ThrowingUsuarioServices : IUsuarioServices
    {
        public Task<bool> CreateUsuarios(CreateUsuariosDTO usuarioCreate) => throw new InvalidOperationException("detalle sensible");

        public Task<ServiceResult<ComponentsFormDTO>> CreateComponentForm(CreateComponentFormDTO dto) => throw new NotImplementedException();

        public Task<ServiceResult<RolesDTO>> CreateRole(CreateRoleDTO dto) => throw new NotImplementedException();

        public Task<IEnumerable<RoleModal>> GetAllRoles() => Task.FromResult(Enumerable.Empty<RoleModal>());

        public Task<IEnumerable<UsuarioDTO>> GetAllUsuarios() => Task.FromResult(Enumerable.Empty<UsuarioDTO>());

        public Task<IEnumerable<ComponentsForm>> GetComponentsForms() => Task.FromResult(Enumerable.Empty<ComponentsForm>());

        public Task<IEnumerable<PermisosModal>> GetPermisosList() => Task.FromResult(Enumerable.Empty<PermisosModal>());

        public Task<IEnumerable<RolesDTO>> GetRolesList() => Task.FromResult(Enumerable.Empty<RolesDTO>());

        public Task<bool> PermisosRoleCreate(RolesPermisoDTO rolesPermisos) => Task.FromResult(true);

        public Task<IEnumerable<PermisosDTO>> PermisosRoleList(int rolId, string componentsId) => Task.FromResult(Enumerable.Empty<PermisosDTO>());

        public Task<IEnumerable<PermisosDTO>> PermisosRoleListAsignacion(int rolId, int componentsId) => Task.FromResult(Enumerable.Empty<PermisosDTO>());

        public Task<ServiceResult<RolesDTO>> UpdateRole(int roleId, UpdateRoleDTO dto) => throw new NotImplementedException();
    }

    private sealed class FakeUsuarioServices : IUsuarioServices
    {
        public Task<bool> CreateUsuarios(CreateUsuariosDTO usuarioCreate) => Task.FromResult(true);

        public Task<ServiceResult<ComponentsFormDTO>> CreateComponentForm(CreateComponentFormDTO dto)
        {
            return Task.FromResult(ServiceResult<ComponentsFormDTO>.Ok(new ComponentsFormDTO
            {
                ComponentsId = 12,
                ComponentsName = dto.ComponentsName ?? string.Empty
            }));
        }

        public Task<ServiceResult<RolesDTO>> CreateRole(CreateRoleDTO dto)
        {
            return Task.FromResult(ServiceResult<RolesDTO>.Ok(new RolesDTO
            {
                RoleId = 10,
                RoleName = dto.RoleName ?? string.Empty
            }));
        }

        public Task<IEnumerable<RoleModal>> GetAllRoles() => Task.FromResult(Enumerable.Empty<RoleModal>());

        public Task<IEnumerable<UsuarioDTO>> GetAllUsuarios() => Task.FromResult(Enumerable.Empty<UsuarioDTO>());

        public Task<IEnumerable<ComponentsForm>> GetComponentsForms() => Task.FromResult(Enumerable.Empty<ComponentsForm>());

        public Task<IEnumerable<PermisosModal>> GetPermisosList() => Task.FromResult(Enumerable.Empty<PermisosModal>());

        public Task<IEnumerable<RolesDTO>> GetRolesList() => Task.FromResult(Enumerable.Empty<RolesDTO>());

        public Task<bool> PermisosRoleCreate(RolesPermisoDTO rolesPermisos) => Task.FromResult(true);

        public Task<IEnumerable<PermisosDTO>> PermisosRoleList(int rolId, string componentsId) => Task.FromResult(Enumerable.Empty<PermisosDTO>());

        public Task<IEnumerable<PermisosDTO>> PermisosRoleListAsignacion(int rolId, int componentsId) => Task.FromResult(Enumerable.Empty<PermisosDTO>());

        public Task<ServiceResult<RolesDTO>> UpdateRole(int roleId, UpdateRoleDTO dto)
        {
            if (roleId == 404)
                return Task.FromResult(ServiceResult<RolesDTO>.Fail("Rol no encontrado", "not_found"));

            return Task.FromResult(ServiceResult<RolesDTO>.Ok(new RolesDTO
            {
                RoleId = roleId,
                RoleName = dto.RoleName ?? string.Empty
            }));
        }
    }
}
