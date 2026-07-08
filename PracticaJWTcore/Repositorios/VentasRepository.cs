using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Dtos.Ventas;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    // Repository de ventas: concentra EF Core para ventas, detalles, articulos y movimientos.
    public class VentasRepository : IVentasRepository
    {
        private readonly PracticaJWTcoreContext _context;

        public VentasRepository(PracticaJWTcoreContext context)
        {
            _context = context;
        }

        public Task<List<Articulos>> GetArticulosByIds(IEnumerable<int> ids)
        {
            var idList = ids.Distinct().ToList();
            return _context.Articulos
                .Where(a => idList.Contains(a.IdArticulo))
                .ToListAsync();
        }

        public async Task<T> ExecuteInTransaction<T>(Func<Task<T>> operation)
        {
            // Permite que el service ejecute varias escrituras como una sola unidad atomica.
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var result = await operation();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task AddVenta(Venta venta)
        {
            await _context.Ventas.AddAsync(venta);
        }

        public async Task AddVentaDetalle(VentaDetalle detalle)
        {
            await _context.VentaDetalles.AddAsync(detalle);
        }

        public async Task AddStockMovimiento(StockMovimiento movimiento)
        {
            await _context.StockMovimientos.AddAsync(movimiento);
        }

        public Task SaveChanges()
        {
            return _context.SaveChangesAsync();
        }

        public Task<VentaResponseDto?> GetVentaResponseById(long id)
        {
            return _context.Ventas
                .Where(v => v.IdVenta == id)
                .Select(v => new VentaResponseDto
                {
                    IdVenta = v.IdVenta,
                    FechaVenta = v.FechaVenta,
                    IdCliente = v.IdCliente,
                    Total = v.Total,
                    Detalles = _context.VentaDetalles
                        .Where(d => d.IdVenta == v.IdVenta)
                        .Select(d => new VentaDetalleResponseDto
                        {
                            IdVentaDetalle = d.IdVentaDetalle,
                            IdVenta = d.IdVenta,
                            IdArticulo = d.IdArticulo,
                            Cantidad = d.Cantidad,
                            PrecioUnitario = d.PrecioUnitario,
                            PorcentajeIva = d.PorcentajeIva,
                            SubTotal = d.SubTotal
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public Task<List<VentaResponseDto>> GetVentas()
        {
            // Proyecta entidades a DTOs para evitar devolver el grafo EF completo al frontend.
            return _context.Ventas
                .Select(v => new VentaResponseDto
                {
                    IdVenta = v.IdVenta,
                    NombreVenta = "Venta " + v.IdVenta,
                    FechaVenta = v.FechaVenta,
                    IdCliente = v.IdCliente,
                    NombreCliente = _context.Customer
                        .Where(c => c.Id == v.IdCliente)
                        .Select(c => c.FirstName)
                        .FirstOrDefault(),
                    IdUsuario = v.IdUsuario,
                    NombreUsuario = _context.Usuarios
                        .Where(u => u.IdUsuario == v.IdUsuario)
                        .Select(u => u.correo)
                        .FirstOrDefault(),
                    SubTotal = v.SubTotal,
                    IvaTotal = v.IvaTotal,
                    Total = v.Total,
                    MetodoPago = v.MetodoPago,
                    Estado = v.Estado,
                    FechaAnulacion = v.FechaAnulacion,
                    MotivoAnulacion = v.MotivoAnulacion,
                    VentaDetalles = v.VentaDetalles.Select(d => new VentaDetalleResponseDto
                    {
                        IdVentaDetalle = d.IdVentaDetalle,
                        IdVenta = d.IdVenta,
                        IdArticulo = d.IdArticulo,
                        NombreArticulo = _context.Articulos
                            .Where(a => a.IdArticulo == d.IdArticulo)
                            .Select(a => a.NombreArticulo)
                            .FirstOrDefault(),
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        PorcentajeIva = d.PorcentajeIva,
                        SubTotal = d.SubTotal
                    }).ToList()
                })
                .ToListAsync();
        }

        public Task<Venta?> GetVentaWithDetalles(long id)
        {
            // Include carga los detalles necesarios para restaurar stock al eliminar la venta.
            return _context.Ventas
                .Include(v => v.VentaDetalles)
                .FirstOrDefaultAsync(v => v.IdVenta == id);
        }

        public Task<Venta?> GetVentaById(long id)
        {
            return _context.Ventas.FirstOrDefaultAsync(v => v.IdVenta == id);
        }

        public Task<bool> ExistsCliente(long idCliente)
        {
            return _context.Customer.AnyAsync(c => c.Id == idCliente);
        }

        public Task<bool> ExistsUsuario(int idUsuario)
        {
            return _context.Usuarios.AnyAsync(u => u.IdUsuario == idUsuario);
        }

        public Task<List<StockMovimiento>> GetMovimientosByReferencia(string referencia)
        {
            return _context.StockMovimientos
                .Where(m => m.Referencia == referencia)
                .ToListAsync();
        }

        public void RemoveStockMovimientos(IEnumerable<StockMovimiento> movimientos)
        {
            _context.StockMovimientos.RemoveRange(movimientos);
        }

        public void RemoveVenta(Venta venta)
        {
            _context.Ventas.Remove(venta);
        }
    }
}
