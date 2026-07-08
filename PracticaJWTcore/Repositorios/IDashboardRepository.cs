using PracticaJWTcore.Dtos.Dashboard;

namespace PracticaJWTcore.Repositorios
{
    public interface IDashboardRepository
    {
        Task<DashboardResumenVentasDto> GetResumenVentas();
        Task<List<DashboardVentaPorArticuloDto>> GetVentasPorArticulo(DateTime? fechaDesde, DateTime? fechaHasta);
        Task<List<DashboardProductoMasVendidoDto>> GetProductosMasVendidos(DateTime? fechaDesde, DateTime? fechaHasta);
        Task<List<DashboardStockBajoDto>> GetStockBajo();
    }
}
