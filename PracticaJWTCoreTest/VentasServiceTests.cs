using PracticaJWTcore.Dtos.Ventas;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;
using PracticaJWTcore.Services;

namespace PracticaJWTCoreTest;

public class VentasServiceTests
{
    // Estas pruebas protegen reglas de negocio de ventas sin depender de EF Core real.
    [Fact]
    public async Task CreateVenta_rechaza_items_vacios()
    {
        var service = new VentasService(new FakeVentasRepository());

        var result = await service.CreateVenta(new CreateVentaDto());

        Assert.False(result.Success);
        Assert.Equal("ITEMS_REQUIRED", result.Code);
    }

    [Fact]
    public async Task CreateVenta_rechaza_stock_insuficiente_sin_abrir_transaccion()
    {
        var repository = new FakeVentasRepository
        {
            Articulos =
            [
                new Articulos
                {
                    IdArticulo = 10,
                    NombreArticulo = "Jabon",
                    PrecioVenta = 100,
                    StockActual = 1
                }
            ]
        };
        var service = new VentasService(repository);

        var result = await service.CreateVenta(new CreateVentaDto
        {
            Items =
            [
                new VentaItemDto
                {
                    ArticuloId = 10,
                    Cantidad = 2
                }
            ]
        });

        Assert.False(result.Success);
        Assert.Equal("STOCK_INSUFFICIENT", result.Code);
        Assert.False(repository.TransactionStarted);
    }

    private sealed class FakeVentasRepository : IVentasRepository
    {
        public List<Articulos> Articulos { get; init; } = [];
        public bool TransactionStarted { get; private set; }

        public Task<List<Articulos>> GetArticulosByIds(IEnumerable<int> ids)
        {
            var idSet = ids.ToHashSet();
            return Task.FromResult(Articulos.Where(a => idSet.Contains(a.IdArticulo)).ToList());
        }

        public Task<T> ExecuteInTransaction<T>(Func<Task<T>> operation)
        {
            TransactionStarted = true;
            return operation();
        }

        public Task AddVenta(Venta venta)
        {
            venta.IdVenta = 1;
            return Task.CompletedTask;
        }

        public Task AddVentaDetalle(VentaDetalle detalle) => Task.CompletedTask;

        public Task AddStockMovimiento(StockMovimiento movimiento) => Task.CompletedTask;

        public Task SaveChanges() => Task.CompletedTask;

        public Task<VentaResponseDto?> GetVentaResponseById(long id) => Task.FromResult<VentaResponseDto?>(null);

        public Task<List<VentaResponseDto>> GetVentas() => Task.FromResult(new List<VentaResponseDto>());

        public Task<Venta?> GetVentaWithDetalles(long id) => Task.FromResult<Venta?>(null);

        public Task<Venta?> GetVentaById(long id) => Task.FromResult<Venta?>(null);

        public Task<bool> ExistsCliente(long idCliente) => Task.FromResult(true);

        public Task<bool> ExistsUsuario(int idUsuario) => Task.FromResult(true);

        public Task<List<StockMovimiento>> GetMovimientosByReferencia(string referencia) => Task.FromResult(new List<StockMovimiento>());

        public void RemoveStockMovimientos(IEnumerable<StockMovimiento> movimientos)
        {
        }

        public void RemoveVenta(Venta venta)
        {
        }
    }
}
