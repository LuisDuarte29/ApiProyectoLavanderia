using PracticaJWTcore.Dtos.Dashboard;
using PracticaJWTcore.Controllers;
using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Repositorios;
using PracticaJWTcore.Services;

namespace PracticaJWTCoreTest;

public class DashboardServiceTests
{
    [Fact]
    public async Task ResumenVentas_devuelve_totales_calculados_por_repository()
    {
        var esperado = new DashboardResumenVentasDto
        {
            TotalVentasDia = 1000,
            TotalVentasMes = 3000,
            CantidadVentasDia = 1,
            CantidadVentasMes = 2,
            CantidadArticulosVendidosDia = 3,
            CantidadArticulosVendidosMes = 8,
            ProductoMasVendido = "Coca Cola 500ml"
        };
        var service = new DashboardService(new FakeDashboardRepository { Resumen = esperado });

        var resultado = await service.GetResumenVentas();

        Assert.Same(esperado, resultado);
    }

    [Fact]
    public async Task ProductosMasVendidos_delega_fechas_y_ranking_al_repository()
    {
        var fechaDesde = new DateTime(2026, 5, 1);
        var fechaHasta = new DateTime(2026, 5, 31);
        var esperado = new List<DashboardProductoMasVendidoDto>
        {
            new()
            {
                IdArticulo = 10,
                NombreArticulo = "Coca Cola 500ml",
                CantidadVendida = 35,
                TotalVendido = 280000,
                StockActual = 20
            }
        };
        var repository = new FakeDashboardRepository { Productos = esperado };
        var service = new DashboardService(repository);

        var resultado = await service.GetProductosMasVendidos(fechaDesde, fechaHasta);

        Assert.Same(esperado, resultado);
        Assert.Equal(fechaDesde, repository.FechaDesde);
        Assert.Equal(fechaHasta, repository.FechaHasta);
    }

    [Fact]
    public async Task VentasPorArticulo_rechaza_rango_de_fechas_invalido()
    {
        var service = new FakeDashboardService();
        var controller = new DashboardController(service);

        var resultado = await controller.GetVentasPorArticulo(new DateTime(2026, 5, 31), new DateTime(2026, 5, 1));

        Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.False(service.VentasPorArticuloConsultado);
    }

    private sealed class FakeDashboardRepository : IDashboardRepository
    {
        public DashboardResumenVentasDto Resumen { get; init; } = new();
        public List<DashboardProductoMasVendidoDto> Productos { get; init; } = [];
        public DateTime? FechaDesde { get; private set; }
        public DateTime? FechaHasta { get; private set; }

        public Task<DashboardResumenVentasDto> GetResumenVentas()
        {
            return Task.FromResult(Resumen);
        }

        public Task<List<DashboardVentaPorArticuloDto>> GetVentasPorArticulo(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            return Task.FromResult(new List<DashboardVentaPorArticuloDto>());
        }

        public Task<List<DashboardProductoMasVendidoDto>> GetProductosMasVendidos(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            FechaDesde = fechaDesde;
            FechaHasta = fechaHasta;
            return Task.FromResult(Productos);
        }

        public Task<List<DashboardStockBajoDto>> GetStockBajo()
        {
            return Task.FromResult(new List<DashboardStockBajoDto>());
        }
    }

    private sealed class FakeDashboardService : IDashboardService
    {
        public bool VentasPorArticuloConsultado { get; private set; }

        public Task<DashboardResumenVentasDto> GetResumenVentas()
        {
            return Task.FromResult(new DashboardResumenVentasDto());
        }

        public Task<List<DashboardVentaPorArticuloDto>> GetVentasPorArticulo(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            VentasPorArticuloConsultado = true;
            return Task.FromResult(new List<DashboardVentaPorArticuloDto>());
        }

        public Task<List<DashboardProductoMasVendidoDto>> GetProductosMasVendidos(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            return Task.FromResult(new List<DashboardProductoMasVendidoDto>());
        }

        public Task<List<DashboardStockBajoDto>> GetStockBajo()
        {
            return Task.FromResult(new List<DashboardStockBajoDto>());
        }
    }
}
