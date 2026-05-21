using PracticaJWTcore.Dtos.Ventas;

namespace PracticaJWTcore.Services
{
    public interface IVentasService
    {
        Task<ServiceResult<VentaResponseDto>> CreateVenta(CreateVentaDto dto);
        Task<List<VentaResponseDto>> GetVentas();
        Task<VentaResponseDto?> GetVenta(long id);
        Task<ServiceResult<object>> UpdateVenta(long id, UpdateVentaDto dto);
        Task<ServiceResult<object>> DeleteVenta(long id);
    }
}
