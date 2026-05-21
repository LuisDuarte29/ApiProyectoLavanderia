using PracticaJWTcore.Dtos.Articulos;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;
using PracticaJWTcore.Services;

namespace PracticaJWTCoreTest;

public class ArticulosServiceTests
{
    // Estas pruebas verifican validaciones del catalogo y restricciones de eliminacion.
    [Fact]
    public async Task Create_rechaza_nombre_vacio()
    {
        var service = new ArticulosService(new FakeArticulosRepository());

        var result = await service.Create(new ArticuloRequestDto());

        Assert.False(result.Success);
        Assert.Equal("ARTICLE_NAME_REQUIRED", result.Code);
    }

    [Fact]
    public async Task Delete_rechaza_articulo_con_ventas_o_movimientos()
    {
        var service = new ArticulosService(new FakeArticulosRepository
        {
            Articulo = new Articulos { IdArticulo = 3, NombreArticulo = "Jabon" },
            HasDependencies = true
        });

        var result = await service.Delete(3);

        Assert.False(result.Success);
        Assert.Equal("ARTICLE_HAS_DEPENDENCIES", result.Code);
    }

    private sealed class FakeArticulosRepository : IArticulosRepository
    {
        public Articulos? Articulo { get; init; }
        public bool HasDependencies { get; init; }

        public Task<List<ArticuloResponseDto>> GetAll() => Task.FromResult(new List<ArticuloResponseDto>());

        public Task<ArticuloResponseDto?> GetById(int id) => Task.FromResult<ArticuloResponseDto?>(null);

        public Task<Articulos?> GetEntityById(int id) => Task.FromResult(Articulo?.IdArticulo == id ? Articulo : null);

        public Task<bool> CategoriaExists(int idCategoria) => Task.FromResult(true);

        public Task<bool> HasVentasOrMovimientos(int idArticulo) => Task.FromResult(HasDependencies);

        public Task Add(Articulos articulo)
        {
            articulo.IdArticulo = 1;
            return Task.CompletedTask;
        }

        public void Remove(Articulos articulo)
        {
        }

        public Task SaveChanges() => Task.CompletedTask;
    }
}
