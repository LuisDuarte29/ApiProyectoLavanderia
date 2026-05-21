using PracticaJWTcore.Dtos.Stock;

namespace PracticaJWTcore.Services
{
    public interface IStockService
    {
        Task<List<StockMovimientoResponseDto>> GetMovimientos();
        Task<StockMovimientoResponseDto?> GetMovimiento(long id);
        Task<ServiceResult<StockMovimientoResponseDto>> CreateMovimiento(StockMovimientoRequestDto movimiento);
        Task<ServiceResult<StockMovimientoResponseDto>> UpdateMovimiento(long id, StockMovimientoRequestDto movimiento);
        Task<ServiceResult<object>> DeleteMovimiento(long id);
    }
}
