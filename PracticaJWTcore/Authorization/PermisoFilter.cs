using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Authorization
{
    public sealed class PermisoFilter : IAsyncAuthorizationFilter
    {
        private readonly PracticaJWTcoreContext _context;
        private readonly string _componente;
        private readonly string _permiso;

        public PermisoFilter(PracticaJWTcoreContext context, string componente, string permiso)
        {
            _context = context;
            _componente = componente;
            _permiso = permiso;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var roleIdValue = context.HttpContext.User.FindFirst("roleId")?.Value;
            if (!int.TryParse(roleIdValue, out var roleId))
            {
                context.Result = new ForbidResult();
                return;
            }

            var tienePermiso = await (from rp in _context.RolesPermisos.AsNoTracking()
                                      join cf in _context.ComponentsForm.AsNoTracking() on rp.ComponentsId equals cf.ComponentsId
                                      join p in _context.Permisos.AsNoTracking() on rp.PermisoId equals p.PermisoId
                                      where rp.RoleId == roleId
                                            && cf.ComponentsName == _componente
                                            && p.PermisoNombre == _permiso
                                      select rp.RolePermisoId)
                .AnyAsync();

            if (!tienePermiso)
                context.Result = new ForbidResult();
        }
    }
}
