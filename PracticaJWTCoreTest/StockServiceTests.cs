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
    public async Task CreateMovimiento_entrada_actualiza_stock_actual_y_calcula_saldos()
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
        Assert.Equal(8, repository.Articulo.StockActual);
        Assert.Equal(5, repository.Movimiento!.StockAnterior);
        Assert.Equal(8, repository.Movimiento.StockNuevo);
        Assert.True(repository.TransactionStarted);
    }

    [Fact]
    public async Task CreateMovimiento_salida_rechaza_stock_insuficiente()
    {
        var repository = new FakeStockRepository
        {
            Articulo = new Articulos
            {
                IdArticulo = 1,
                StockActual = 2,
                NombreArticulo = "Jabon"
            }
        };
        var service = new StockService(repository);

        var result = await service.CreateMovimiento(new StockMovimientoRequestDto
        {
            IdArticulo = 1,
            TipoMovimiento = "Salida",
            Cantidad = 3
        });

        Assert.False(result.Success);
        Assert.Equal("STOCK_INSUFFICIENT", result.Code);
        Assert.Equal(2, repository.Articulo.StockActual);
    }
    [Fact]
    public async Task CreateMovimiento_salida_descuenta_stock_y_registra_movimiento()
    {
        var repository = new FakeStockRepository()
        {
            Articulo = new Articulos
            {
                IdArticulo = 1,
                StockActual = 10,
            }
        };

        var service=new StockService(repository);

        var descuentoMovimiento = await service.CreateMovimiento(new StockMovimientoRequestDto
        {
            TipoMovimiento = "Salida",
            Cantidad = 4,
            IdArticulo=1
        });

        var articulorestante = await repository.GetArticulo(repository.Articulo.IdArticulo);

        Assert.NotNull(repository);
        Assert.NotNull(service);
        Assert.NotNull(descuentoMovimiento);
        Assert.NotNull(articulorestante);
        Assert.Equal(6,articulorestante?.StockActual);
        Assert.Equal(1, repository.Articulo.IdArticulo);
        Assert.Equal("Salida", repository?.Movimiento?.TipoMovimiento);



    }

    private sealed class FakeStockRepository : IStockRepository
    {
        public Articulos? Articulo { get; init; }
        public StockMovimiento? Movimiento { get; private set; }
        public bool TransactionStarted { get; private set; }

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

        public Task<T> ExecuteInTransaction<T>(Func<Task<T>> operation)
        {
            TransactionStarted = true;
            return operation();
        }
    }
}
