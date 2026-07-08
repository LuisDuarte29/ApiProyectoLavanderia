using PracticaJWTcore.Dtos.Dashboard;
using PracticaJWTcore.Repositorios;

namespace PracticaJWTcore.Services
{
    // Service del dashboard: mantiene el controller libre de consultas y reglas de agregacion.
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repository;

        public DashboardService(IDashboardRepository repository)
        {
            _repository = repository;
        }

        public Task<DashboardResumenVentasDto> GetResumenVentas()
        {
            return _repository.GetResumenVentas();
        }

        public Task<List<DashboardVentaPorArticuloDto>> GetVentasPorArticulo(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            return _repository.GetVentasPorArticulo(fechaDesde, fechaHasta);
        }

        public Task<List<DashboardProductoMasVendidoDto>> GetProductosMasVendidos(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            return _repository.GetProductosMasVendidos(fechaDesde, fechaHasta);
        }

        public Task<List<DashboardStockBajoDto>> GetStockBajo()
        {
            return _repository.GetStockBajo();
        }
    }
}
