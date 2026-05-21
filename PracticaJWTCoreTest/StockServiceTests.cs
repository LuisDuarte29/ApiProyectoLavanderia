using PracticaJWTcore.Dtos.Stock;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;
using PracticaJWTcore.Services;

namespace PracticaJWTCoreTest;

public class StockServiceTests
{
    // Estas pruebas documentan el comportamiento actual de movimientos manuales de stock.
    [Fact]
    public async Task CreateMovimiento_rechaza_cantidad_no_positiva()
    {
        var service = new StockService(new FakeStockRepository());

        var result = await service.CreateMovimiento(new StockMovimientoRequestDto
        {
            IdArticulo = 1,
            Cantidad = 0
        });

        Assert.False(result.Success);
        Assert.Equal("INVALID_QUANTITY", result.Code);
    }

    [Fact]
    public async Task CreateMovimiento_no_actualiza_stock_actual_para_preservar_comportamiento_manual()
    {
        var repository = new FakeStockRepository
        {
            Articulo = new Articulos
            {
                IdArticulo = 1,
                StockActual = 5,
                NombreArticulo = "Jabon"
            }
        };
        var service = new StockService(repository);

        var result = await service.CreateMovimiento(new StockMovimientoRequestDto
        {
            IdArticulo = 1,
            TipoMovimiento = "Entrada",
            Cantidad = 3
        });

        Assert.True(result.Success);
        Assert.Equal(5, repository.Articulo.StockActual);
    }

    private sealed class FakeStockRepository : IStockRepository
    {
        public Articulos? Articulo { get; init; }
        public StockMovimiento? Movimiento { get; private set; }

        public Task<Articulos?> GetArticulo(int idArticulo)
        {
            return Task.FromResult(Articulo?.IdArticulo == idArticulo ? Articulo : null);
        }

        public Task<List<StockMovimientoResponseDto>> GetMovimientos()
        {
            return Task.FromResult(new List<StockMovimientoResponseDto>());
        }

        public Task<StockMovimientoResponseDto?> GetMovimiento(long id)
        {
            return Task.FromResult<StockMovimientoResponseDto?>(null);
        }

        public Task<StockMovimiento?> GetMovimientoEntity(long id)
        {
            return Task.FromResult(Movimiento);
        }

        public Task AddMovimiento(StockMovimiento movimiento)
        {
            movimiento.IdStockMovimiento = 1;
            Movimiento = movimiento;
            return Task.CompletedTask;
        }

        public void RemoveMovimiento(StockMovimiento movimiento)
        {
            Movimiento = null;
        }

        public Task SaveChanges() => Task.CompletedTask;
    }
}
