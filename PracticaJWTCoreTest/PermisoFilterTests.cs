using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Authorization;
using PracticaJWTcore.Models;

namespace PracticaJWTCoreTest;

public class PermisoFilterTests
{
    [Fact]
    public async Task OnAuthorizationAsync_permiso_existente_deja_pasar()
    {
        var options = new DbContextOptionsBuilder<PracticaJWTcoreContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new PracticaJWTcoreContext(options);
        context.ComponentsForm.Add(new ComponentsForm { ComponentsId = 1, ComponentsName = "ListaProductos" });
        context.Permisos.Add(new Permisos { PermisoId = 2, PermisoNombre = "Leer" });
        context.RolesPermisos.Add(new RolesPermisos { RolePermisoId = 1, RoleId = 7, ComponentsId = 1, PermisoId = 2 });
        await context.SaveChangesAsync();

        var filter = new PermisoFilter(context, "ListaProductos", "Leer");
        var authorizationContext = BuildContext("7");

        await filter.OnAuthorizationAsync(authorizationContext);

        Assert.Null(authorizationContext.Result);
    }

    [Fact]
    public async Task OnAuthorizationAsync_permiso_faltante_devuelve_forbid()
    {
        var options = new DbContextOptionsBuilder<PracticaJWTcoreContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new PracticaJWTcoreContext(options);
        var filter = new PermisoFilter(context, "ListaProductos", "Eliminar");
        var authorizationContext = BuildContext("7");

        await filter.OnAuthorizationAsync(authorizationContext);

        Assert.IsType<ForbidResult>(authorizationContext.Result);
    }

    private static AuthorizationFilterContext BuildContext(string roleId)
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim("roleId", roleId)
            ], "Test"))
        };

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        return new AuthorizationFilterContext(actionContext, []);
    }
}
