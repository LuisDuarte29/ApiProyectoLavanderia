using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Dtos.Stock;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    // Repository de stock: accede a StockMovimientos y Articulos con EF Core.
    public class StockRepository : IStockRepository
    {
        private readonly PracticaJWTcoreContext _context;

        public StockRepository(PracticaJWTcoreContext context)
        {
            _context = context;
        }

        public Task<Articulos?> GetArticulo(int idArticulo)
        {
            return _context.Articulos.FirstOrDefaultAsync(a => a.IdArticulo == idArticulo);
        }

        public Task<List<StockMovimientoResponseDto>> GetMovimientos()
        {
            // La consulta proyecta el nombre del articulo para que el frontend no haga otra llamada.
            return _context.StockMovimientos
                .OrderByDescending(m => m.FechaMovimiento)
                .Select(m => new StockMovimientoResponseDto
                {
                    IdStockMovimiento = m.IdStockMovimiento,
                    FechaMovimiento = m.FechaMovimiento,
                    IdArticulo = m.IdArticulo,
                    NombreArticulo = _context.Articulos
                        .Where(a => a.IdArticulo == m.IdArticulo)
                        .Select(a => a.NombreArticulo)
                        .FirstOrDefault(),
                    TipoMovimiento = m.TipoMovimiento,
                    Cantidad = m.Cantidad,
                    StockAnterior = m.StockAnterior,
                    StockNuevo = m.StockNuevo,
                    Referencia = m.Referencia,
                    Observacion = m.Observacion
                })
                .ToListAsync();
        }

        public Task<StockMovimientoResponseDto?> GetMovimiento(long id)
        {
            return _context.StockMovimientos
                .Where(m => m.IdStockMovimiento == id)
                .Select(m => new StockMovimientoResponseDto
                {
                    IdStockMovimiento = m.IdStockMovimiento,
                    FechaMovimiento = m.FechaMovimiento,
                    IdArticulo = m.IdArticulo,
                    NombreArticulo = _context.Articulos
                        .Where(a => a.IdArticulo == m.IdArticulo)
                        .Select(a => a.NombreArticulo)
                        .FirstOrDefault(),
                    TipoMovimiento = m.TipoMovimiento,
                    Cantidad = m.Cantidad,
                    StockAnterior = m.StockAnterior,
                    StockNuevo = m.StockNuevo,
                    Referencia = m.Referencia,
                    Observacion = m.Observacion
                })
                .FirstOrDefaultAsync();
        }

        public Task<StockMovimiento?> GetMovimientoEntity(long id)
        {
            return _context.StockMovimientos.FirstOrDefaultAsync(m => m.IdStockMovimiento == id);
        }

        public async Task AddMovimiento(StockMovimiento movimiento)
        {
            await _context.StockMovimientos.AddAsync(movimiento);
        }

        public void RemoveMovimiento(StockMovimiento movimiento)
        {
            _context.StockMovimientos.Remove(movimiento);
        }

        public Task SaveChanges()
        {
            return _context.SaveChangesAsync();
        }
    }
}
