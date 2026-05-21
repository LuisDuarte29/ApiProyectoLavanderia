using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Models;

namespace PracticaJWTCoreTest;

public class EfModelMappingTests
{
    [Fact]
    public void Roles_no_debe_generar_columna_sombra_de_usuario()
    {
        var options = new DbContextOptionsBuilder<PracticaJWTcoreContext>()
            .UseSqlServer("Server=(local);Database=ApiProyecto;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        using var context = new PracticaJWTcoreContext(options);
        var roleEntity = context.Model.FindEntityType(typeof(Roles));

        Assert.NotNull(roleEntity);
        Assert.DoesNotContain(roleEntity!.GetProperties(), p => p.Name == "usuarioIdUsuario");
    }
}
