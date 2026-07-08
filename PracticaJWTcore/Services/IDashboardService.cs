using PracticaJWTcore.Dtos.Dashboard;

namespace PracticaJWTcore.Services
{
    public interface IDashboardService
    {
        Task<DashboardResumenVentasDto> GetResumenVentas();
        Task<List<DashboardVentaPorArticuloDto>> GetVentasPorArticulo(DateTime? fechaDesde, DateTime? fechaHasta);
        Task<List<DashboardProductoMasVendidoDto>> GetProductosMasVendidos(DateTime? fechaDesde, DateTime? fechaHasta);
        Task<List<DashboardStockBajoDto>> GetStockBajo();
    }
}
