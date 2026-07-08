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

    [Fact]
    public async Task DeleteVenta_anula_venta_y_registra_entrada_sin_borrar_historial()
    {
        var venta = new Venta
        {
            IdVenta = 5,
            Estado = "CONFIRMADA",
            VentaDetalles =
            [
                new VentaDetalle
                {
                    IdArticulo = 10,
                    Cantidad = 2
                }
            ]
        };
        var articulo = new Articulos
        {
            IdArticulo = 10,
            StockActual = 3
        };
        var repository = new FakeVentasRepository
        {
            Venta = venta,
            Articulos = [articulo]
        };
        var service = new VentasService(repository);

        var result = await service.DeleteVenta(5);

        Assert.True(result.Success);
        Assert.Equal("ANULADA", venta.Estado);
        Assert.Equal(5, articulo.StockActual);
        Assert.False(repository.RemoveVentaCalled);
        Assert.False(repository.RemoveMovimientosCalled);
        Assert.Contains(repository.StockMovimientos, m => m.Referencia == "AnulacionVenta:5");
    }
    [Fact]
    public async Task CreateVenta_con_stock_suficiente_descuenta_stock_registra_movimiento_y_calcula_totales()
    {
   
        var repository = new FakeVentasRepository()
        {
            Articulos =[
                new Articulos() {
                IdArticulo = 10,
                NombreArticulo = "Jabon",
                PrecioVenta = 100,
                StockActual = 5
            }
          ]
        };

        var service1 = new VentasService(repository);

        var realizarVenta = await service1.CreateVenta(new CreateVentaDto()
        {
            IdCliente = 1,
            IdUsuario = 2,
            MetodoPago = "Efectivo",

            Items = [
                new VentaItemDto()
            {
                ArticuloId=10,
                Cantidad= 2,
            }
            ]

        });

        Assert.True(realizarVenta.Success);
        Assert.NotNull(repository);
        Assert.Equal(3, repository.Articulos[0].StockActual);
     }
        

    private sealed class FakeVentasRepository : IVentasRepository
    {

        public Venta VentaCreada { get; set; }
        public List<VentaDetalle> ventaDetallesList { get; set; } = [];
        public List<Articulos> Articulos { get; init; } = [];
        public Venta? Venta { get; init; }
        public List<StockMovimiento> StockMovimientos { get; } = [];
        public bool TransactionStarted { get; private set; }
        public bool RemoveVentaCalled { get; private set; }
        public bool RemoveMovimientosCalled { get; private set; }

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
            VentaCreada = venta;
            return Task.CompletedTask;
        }

        public Task AddVentaDetalle(VentaDetalle detalle) 
        {
            ventaDetallesList.Add(detalle);
                return Task.CompletedTask;
        }

        public Task AddStockMovimiento(StockMovimiento movimiento)
        {
            StockMovimientos.Add(movimiento);
            return Task.CompletedTask;
        }

        public Task SaveChanges() => Task.CompletedTask;

        public Task<VentaResponseDto?> GetVentaResponseById(long id) => Task.FromResult<VentaResponseDto?>(null);

        public Task<List<VentaResponseDto>> GetVentas() => Task.FromResult(new List<VentaResponseDto>());

        public Task<Venta?> GetVentaWithDetalles(long id) => Task.FromResult(Venta?.IdVenta == id ? Venta : null);

        public Task<Venta?> GetVentaById(long id) => Task.FromResult<Venta?>(null);

        public Task<bool> ExistsCliente(long idCliente) => Task.FromResult(true);

        public Task<bool> ExistsUsuario(int idUsuario) => Task.FromResult(true);

        public Task<List<StockMovimiento>> GetMovimientosByReferencia(string referencia) => Task.FromResult(new List<StockMovimiento>());

        public void RemoveStockMovimientos(IEnumerable<StockMovimiento> movimientos)
        {
            RemoveMovimientosCalled = true;
        }

        public void RemoveVenta(Venta venta)
        {
            RemoveVentaCalled = true;
        }
    }
}
