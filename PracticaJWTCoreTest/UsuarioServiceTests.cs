using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;
using PracticaJWTcore.Services;

namespace PracticaJWTCoreTest;

public class UsuarioServiceTests
{
    [Fact]
    public async Task CreateUsuarios_rechaza_correo_duplicado()
    {
        var service = new UsuarioServices(new FakeUsuariosRepository
        {
            RoleExistsResult = true,
            CorreoExistsResult = true
        });

        var result = await service.CreateUsuarios(new CreateUsuariosDTO
        {
            correo = "admin@test.com",
            clave = "Clave123",
            RoleId = 1
        });

        Assert.False(result);
    }

    private sealed class FakeUsuariosRepository : IUsuariosRepository
    {
        public bool RoleExistsResult { get; init; }
        public bool CorreoExistsResult { get; init; }

        public Task<IEnumerable<UsuarioDTO>> GetAllUsuarios() => Task.FromResult(Enumerable.Empty<UsuarioDTO>());
        public Task<bool> CreateUsuarios(CreateUsuariosDTO usuarioCreate) => Task.FromResult(true);
        public Task<RolesDTO> CreateRole(CreateRoleDTO dto) => Task.FromResult(new RolesDTO());
        public Task<RolesDTO?> UpdateRole(int roleId, UpdateRoleDTO dto) => Task.FromResult<RolesDTO?>(new RolesDTO());
        public Task<bool> RoleExists(int roleId) => Task.FromResult(RoleExistsResult);
        public Task<bool> RoleNameExists(string roleName, int? excludeRoleId = null) => Task.FromResult(false);
        public Task<IEnumerable<RoleModal>> GetAllRoles() => Task.FromResult(Enumerable.Empty<RoleModal>());
        public Task<IEnumerable<RolesDTO>> GetRolesList() => Task.FromResult(Enumerable.Empty<RolesDTO>());
        public Task<IEnumerable<PermisosDTO>> PermisosRoleList(int roleId, string ComponentsId) => Task.FromResult(Enumerable.Empty<PermisosDTO>());
        public Task<IEnumerable<PermisosModal>> GetPermisosList() => Task.FromResult(Enumerable.Empty<PermisosModal>());
        public Task<bool> PermisosRoleCreate(RolesPermisoDTO rolesPermisos) => Task.FromResult(true);
        public Task<IEnumerable<ComponentsForm>> GetComponentsForms() => Task.FromResult(Enumerable.Empty<ComponentsForm>());
        public Task<ComponentsFormDTO> CreateComponentForm(CreateComponentFormDTO dto) => Task.FromResult(new ComponentsFormDTO());
        public Task<bool> ComponentExists(int componentsFormId) => Task.FromResult(true);
        public Task<bool> ComponentNameExists(string componentsName, int? excludeComponentsId = null) => Task.FromResult(false);
        public Task<bool> PermisosExist(IEnumerable<int> permisosId) => Task.FromResult(true);
        public Task<IEnumerable<PermisosDTO>> PermisosRoleListAsignacion(int roleId, int componentsFormId) => Task.FromResult(Enumerable.Empty<PermisosDTO>());
        public Task<bool> UsuarioCorreoExists(string correo, int? excludeUsuarioId = null) => Task.FromResult(CorreoExistsResult);
    }
}
