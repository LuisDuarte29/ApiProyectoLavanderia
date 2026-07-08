using PracticaJWTcore.Dtos.Articulos;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;
using PracticaJWTcore.Services;

namespace PracticaJWTCoreTest;

public class ArticulosServiceTests
{
    // Estas pruebas verifican validaciones del catalogo y restricciones de eliminacion.
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
    [Fact]
    public async Task Nombre_Articulo_Vacio()
    {
        //Lo que hago primero es inicializar el servicio, ya que estoy trabajando con ArticulosService
        //Este servicio necesita un IArticulosRepository, por eso creo FakeArticulosRepository ya que lo que necesito es una clase
        //Dentro de esa clase heredo de IArticulosRepository que es lo que tiene el constructor de ArticulosServices
        var service = new ArticulosService(new FakeArticulosRepository());
        //En el anterior paso yo habia creado el objeto que necesito, en este creo los datos de prueba que necesito 
        //como solo quiero que nombre este vacio solo pongo ese vacio
        var articulosDTO = new ArticuloRequestDto
        {
            NombreArticulo = "",
            IdCategoria = 1,
            Precio = 10.0m
        };
        //En este paso es donde actua o donde ejecuto el metodo, que en este caso es create enviandole como parametro el objeto de prueba

        var createResult = await service.Create(articulosDTO);

        //Este debe ser asi porque asi porque debe ser falso, ya que no debe de crear porque no tiene el nombreArticulo
        Assert.False(createResult.Success);

        //tener en cuenta que este es mas bien para comparar el resultado que esta en serviceResult especifico y comparar con lo que ha 
        //traido en el test
        Assert.Equal("ARTICLE_NAME_REQUIRED", createResult.Code);
    }
    [Fact]
    public async Task Create_rechaza_categoria_inexistente()
    {
        var service = new ArticulosService(new FakeArticulosRepository
        {
            // En este test no se consulta la base real.
            // CategoriaExistsResult = false hace que el fake responda que la categoria no existe,
            // sin importar si el IdCategoria es 99, 7 o cualquier otro numero.
            CategoriaExistsResult = false,
        });
        var articuloDTO = new ArticuloRequestDto()
        {
            NombreArticulo = "Jabon",
            IdCategoria = 99,
            Precio = 10.0m
        };
        var createResult = await service.Create(articuloDTO);
        Assert.False(createResult.Success);

        Assert.Equal("CATEGORY_NOT_FOUND", createResult?.Code);


    }
    [Fact]
    public async Task Create_crea_articulo_cuando_datos_son_validos()
    {

        var service = new ArticulosService(new FakeArticulosRepository());

        var articulosDTO = new ArticuloRequestDto()
        {
            NombreArticulo = "Jabon",
            IdCategoria = 99,
            Precio = 10.0m,
            PrecioCosto = 5,
            PrecioVenta = 10,
            StockActual = 3,
            StockMinimo = 1,
            Activo = true
        };
        var createResult=await service.Create(articulosDTO);

        Assert.True(createResult.Success);

        Assert.Equal("Jabon", createResult.Value.NombreArticulo);
        Assert.NotNull(createResult.Value.IdCategoria);


    }


    private sealed class FakeArticulosRepository : IArticulosRepository
    {
        public bool CategoriaExistsResult { get; init; } = true;
        public Articulos? Articulo { get; init; }
        public bool HasDependencies { get; init; }

        public Task<List<ArticuloResponseDto>> GetAll() => Task.FromResult(new List<ArticuloResponseDto>());

        public Task<ArticuloResponseDto?> GetById(int id) => Task.FromResult<ArticuloResponseDto?>(null);

        public Task<Articulos?> GetEntityById(int id) => Task.FromResult(Articulo?.IdArticulo == id ? Articulo : null);

        //Para entender mejor el Task.FromResult es el posible resultado que debe devolver mi CategoriaExists en este caso
        //Como probe en Create_rechaza_categoria_inexistente que es CategoriaExistsResult=false automaticamente asume eso y me devuelve falso
        public Task<bool> CategoriaExists(int idCategoria) => Task.FromResult(CategoriaExistsResult);

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
