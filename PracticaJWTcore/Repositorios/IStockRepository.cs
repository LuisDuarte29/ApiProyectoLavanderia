using PracticaJWTcore.Dtos.Stock;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public interface IStockRepository
    {
        Task<Articulos?> GetArticulo(int idArticulo);
        Task<List<StockMovimientoResponseDto>> GetMovimientos();
        Task<StockMovimientoResponseDto?> GetMovimiento(long id);
        Task<StockMovimiento?> GetMovimientoEntity(long id);
        Task AddMovimiento(StockMovimiento movimiento);
        void RemoveMovimiento(StockMovimiento movimiento);
        Task SaveChanges();
    }
}
