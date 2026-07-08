using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Dtos.Dashboard;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    // Repository del dashboard: calcula agregados en SQL para evitar mandar ventas completas al frontend.
    public class DashboardRepository : IDashboardRepository
    {
        private const string EstadoVentaConfirmada = "CONFIRMADA";
        private readonly PracticaJWTcoreContext _context;

        public DashboardRepository(PracticaJWTcoreContext context)
        {
            _context = context;
        }

        public async Task<DashboardResumenVentasDto> GetResumenVentas()
        {
            var hoy = DateTime.UtcNow.Date;
            var manana = hoy.AddDays(1);
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
            var inicioMesSiguiente = inicioMes.AddMonths(1);

            var ventasDia = VentasConfirmadas()
                .Where(v => v.FechaVenta >= hoy && v.FechaVenta < manana);
            var ventasMes = VentasConfirmadas()
                .Where(v => v.FechaVenta >= inicioMes && v.FechaVenta < inicioMesSiguiente);

            var totalVentasDia = await ventasDia.SumAsync(v => (decimal?)v.Total) ?? 0m;
            var totalVentasMes = await ventasMes.SumAsync(v => (decimal?)v.Total) ?? 0m;
            var cantidadVentasDia = await ventasDia.CountAsync();
            var cantidadVentasMes = await ventasMes.CountAsync();
            var cantidadArticulosVendidosDia = await CantidadArticulosVendidos(hoy, manana);
            var cantidadArticulosVendidosMes = await CantidadArticulosVendidos(inicioMes, inicioMesSiguiente);
            var productoMasVendido = await ProductoMasVendido(inicioMes, inicioMesSiguiente);

            return new DashboardResumenVentasDto
            {
                TotalVentasDia = totalVentasDia,
                TotalVentasMes = totalVentasMes,
                CantidadVentasDia = cantidadVentasDia,
                CantidadVentasMes = cantidadVentasMes,
                CantidadArticulosVendidosDia = cantidadArticulosVendidosDia,
                CantidadArticulosVendidosMes = cantidadArticulosVendidosMes,
                ProductoMasVendido = productoMasVendido
            };
        }

        public Task<List<DashboardVentaPorArticuloDto>> GetVentasPorArticulo(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var ventas = AplicarFiltroFechas(VentasConfirmadas(), fechaDesde, fechaHasta);

            return QueryVentasPorArticulo(ventas)
                .Select(x => new DashboardVentaPorArticuloDto
                {
                    IdArticulo = x.IdArticulo,
                    NombreArticulo = x.NombreArticulo,
                    CantidadVendida = x.CantidadVendida,
                    TotalVendido = x.TotalVendido
                })
                .ToListAsync();
        }

        public Task<List<DashboardProductoMasVendidoDto>> GetProductosMasVendidos(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var ventas = AplicarFiltroFechas(VentasConfirmadas(), fechaDesde, fechaHasta);

            return QueryVentasPorArticulo(ventas)
                .Select(x => new DashboardProductoMasVendidoDto
                {
                    IdArticulo = x.IdArticulo,
                    NombreArticulo = x.NombreArticulo,
                    CantidadVendida = x.CantidadVendida,
                    TotalVendido = x.TotalVendido,
                    StockActual = x.StockActual
                })
                .ToListAsync();
        }

        public Task<List<DashboardStockBajoDto>> GetStockBajo()
        {
            return _context.Articulos
                .AsNoTracking()
                .Where(a => a.Activo && a.StockActual <= a.StockMinimo)
                .OrderBy(a => a.StockActual)
                .ThenBy(a => a.NombreArticulo)
                .Select(a => new DashboardStockBajoDto
                {
                    IdArticulo = a.IdArticulo,
                    NombreArticulo = a.NombreArticulo,
                    StockActual = a.StockActual,
                    StockMinimo = a.StockMinimo
                })
                .ToListAsync();
        }

        private IQueryable<Venta> VentasConfirmadas()
        {
            return _context.Ventas
                .AsNoTracking()
                .Where(v => v.Estado == EstadoVentaConfirmada);
        }

        private static IQueryable<Venta> AplicarFiltroFechas(IQueryable<Venta> ventas, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            if (fechaDesde.HasValue)
            {
                var desde = fechaDesde.Value.Date;
                ventas = ventas.Where(v => v.FechaVenta >= desde);
            }

            if (fechaHasta.HasValue)
            {
                var hastaExclusivo = fechaHasta.Value.Date.AddDays(1);
                ventas = ventas.Where(v => v.FechaVenta < hastaExclusivo);
            }

            return ventas;
        }

        private async Task<decimal> CantidadArticulosVendidos(DateTime desde, DateTime hastaExclusivo)
        {
            var cantidad = await (from v in VentasConfirmadas()
                                  join d in _context.VentaDetalles.AsNoTracking() on v.IdVenta equals d.IdVenta
                                  where v.FechaVenta >= desde && v.FechaVenta < hastaExclusivo
                                  select (decimal?)d.Cantidad)
                .SumAsync();

            return cantidad ?? 0m;
        }

        private Task<string?> ProductoMasVendido(DateTime desde, DateTime hastaExclusivo)
        {
            return (from v in VentasConfirmadas()
                    join d in _context.VentaDetalles.AsNoTracking() on v.IdVenta equals d.IdVenta
                    join a in _context.Articulos.AsNoTracking() on d.IdArticulo equals a.IdArticulo
                    where v.FechaVenta >= desde && v.FechaVenta < hastaExclusivo
                    group d by new { a.IdArticulo, a.NombreArticulo } into g
                    orderby g.Sum(d => d.Cantidad) descending, g.Key.NombreArticulo
                    select g.Key.NombreArticulo)
                .FirstOrDefaultAsync();
        }

        private IQueryable<DashboardProductoMasVendidoDto> QueryVentasPorArticulo(IQueryable<Venta> ventas)
        {
            return from v in ventas
                   join d in _context.VentaDetalles.AsNoTracking() on v.IdVenta equals d.IdVenta
                   join a in _context.Articulos.AsNoTracking() on d.IdArticulo equals a.IdArticulo
                   group new { d, a } by new { a.IdArticulo, a.NombreArticulo, a.StockActual } into g
                   orderby g.Sum(x => x.d.Cantidad) descending, g.Key.NombreArticulo
                   select new DashboardProductoMasVendidoDto
                   {
                       IdArticulo = g.Key.IdArticulo,
                       NombreArticulo = g.Key.NombreArticulo,
                       CantidadVendida = g.Sum(x => x.d.Cantidad),
                       TotalVendido = g.Sum(x => x.d.SubTotal),
                       StockActual = g.Key.StockActual
                   };
        }
    }
}
