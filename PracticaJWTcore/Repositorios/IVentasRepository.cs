using PracticaJWTcore.Dtos.Ventas;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public interface IVentasRepository
    {
        Task<List<Articulos>> GetArticulosByIds(IEnumerable<int> ids);
        Task<T> ExecuteInTransaction<T>(Func<Task<T>> operation);
        Task AddVenta(Venta venta);
        Task AddVentaDetalle(VentaDetalle detalle);
        Task AddStockMovimiento(StockMovimiento movimiento);
        Task SaveChanges();
        Task<VentaResponseDto?> GetVentaResponseById(long id);
        Task<List<VentaResponseDto>> GetVentas();
        Task<Venta?> GetVentaWithDetalles(long id);
        Task<Venta?> GetVentaById(long id);
        Task<bool> ExistsCliente(long idCliente);
        Task<bool> ExistsUsuario(int idUsuario);
        Task<List<StockMovimiento>> GetMovimientosByReferencia(string referencia);
        void RemoveStockMovimientos(IEnumerable<StockMovimiento> movimientos);
        void RemoveVenta(Venta venta);
    }
}
