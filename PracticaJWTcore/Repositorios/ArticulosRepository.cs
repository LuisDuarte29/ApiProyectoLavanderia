using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Dtos.Articulos;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    // Repository de articulos: encapsula consultas EF Core del catalogo y sus dependencias.
    public class ArticulosRepository : IArticulosRepository
    {
        private readonly PracticaJWTcoreContext _context;

        public ArticulosRepository(PracticaJWTcoreContext context)
        {
            _context = context;
        }

        public Task<List<ArticuloResponseDto>> GetAll()
        {
            // Proyecta a DTO e incluye el nombre de categoria sin exponer navegaciones EF.
            return _context.Articulos
                .Select(a => new ArticuloResponseDto
                {
                    IdArticulo = a.IdArticulo,
                    NombreArticulo = a.NombreArticulo,
                    Precio = a.Precio,
                    Codigo = a.Codigo,
                    CodigoBarra = a.CodigoBarra,
                    Descripcion = a.Descripcion,
                    PrecioCosto = a.PrecioCosto,
                    PrecioVenta = a.PrecioVenta,
                    StockActual = a.StockActual,
                    StockMinimo = a.StockMinimo,
                    Activo = a.Activo,
                    IdCategoria = a.IdCategoria,
                    NombreCategoria = _context.Categorias
                        .Where(c => c.IdCategoria == a.IdCategoria)
                        .Select(c => c.NombreCategoria)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        public Task<ArticuloResponseDto?> GetById(int id)
        {
            return _context.Articulos
                .Where(a => a.IdArticulo == id)
                .Select(a => new ArticuloResponseDto
                {
                    IdArticulo = a.IdArticulo,
                    NombreArticulo = a.NombreArticulo,
                    Precio = a.Precio,
                    Codigo = a.Codigo,
                    CodigoBarra = a.CodigoBarra,
                    Descripcion = a.Descripcion,
                    PrecioCosto = a.PrecioCosto,
                    PrecioVenta = a.PrecioVenta,
                    StockActual = a.StockActual,
                    StockMinimo = a.StockMinimo,
                    Activo = a.Activo,
                    IdCategoria = a.IdCategoria,
                    NombreCategoria = _context.Categorias
                        .Where(c => c.IdCategoria == a.IdCategoria)
                        .Select(c => c.NombreCategoria)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();
        }

        public Task<Articulos?> GetEntityById(int id)
        {
            return _context.Articulos.FirstOrDefaultAsync(a => a.IdArticulo == id);
        }

        public Task<bool> CategoriaExists(int idCategoria)
        {
            return _context.Categorias.AnyAsync(c => c.IdCategoria == idCategoria);
        }

        public async Task<bool> HasVentasOrMovimientos(int idArticulo)
        {
            // Evita borrar articulos que ya tienen trazabilidad en ventas o stock.
            var tieneDetallesVenta = await _context.VentaDetalles.AnyAsync(d => d.IdArticulo == idArticulo);
            var tieneMovimientos = await _context.StockMovimientos.AnyAsync(m => m.IdArticulo == idArticulo);
            return tieneDetallesVenta || tieneMovimientos;
        }

        public async Task Add(Articulos articulo)
        {
            await _context.Articulos.AddAsync(articulo);
        }

        public void Remove(Articulos articulo)
        {
            _context.Articulos.Remove(articulo);
        }

        public Task SaveChanges()
        {
            return _context.SaveChangesAsync();
        }
    }
}
