using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Authorization;
using PracticaJWTcore.Services;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Authorize]
    [Permiso("Dashboard", "Leer")]
    [Route("api/dashboard")]
    // Controller del dashboard: expone datos ya resumidos para que el frontend solo consuma y muestre.
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("resumen-ventas")]
        public async Task<IActionResult> GetResumenVentas()
        {
            var resumen = await _dashboardService.GetResumenVentas();
            return Ok(resumen);
        }

        [HttpGet("ventas-por-articulo")]
        public async Task<IActionResult> GetVentasPorArticulo([FromQuery] DateTime? fechaDesde, [FromQuery] DateTime? fechaHasta)
        {
            if (FechaDesdeMayorQueFechaHasta(fechaDesde, fechaHasta))
                return BadRequest("fechaDesde no puede ser mayor que fechaHasta");

            var ventasPorArticulo = await _dashboardService.GetVentasPorArticulo(fechaDesde, fechaHasta);
            return Ok(ventasPorArticulo);
        }

        [HttpGet("productos-mas-vendidos")]
        public async Task<IActionResult> GetProductosMasVendidos([FromQuery] DateTime? fechaDesde, [FromQuery] DateTime? fechaHasta)
        {
            if (FechaDesdeMayorQueFechaHasta(fechaDesde, fechaHasta))
                return BadRequest("fechaDesde no puede ser mayor que fechaHasta");

            var productos = await _dashboardService.GetProductosMasVendidos(fechaDesde, fechaHasta);
            return Ok(productos);
        }

        [HttpGet("stock-bajo")]
        public async Task<IActionResult> GetStockBajo()
        {
            var stockBajo = await _dashboardService.GetStockBajo();
            return Ok(stockBajo);
        }

        private static bool FechaDesdeMayorQueFechaHasta(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            return fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde.Value.Date > fechaHasta.Value.Date;
        }
    }
}
