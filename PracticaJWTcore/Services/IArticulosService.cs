using PracticaJWTcore.Dtos.Articulos;

namespace PracticaJWTcore.Services
{
    public interface IArticulosService
    {
        Task<List<ArticuloResponseDto>> GetAll();
        Task<ArticuloResponseDto?> GetById(int id);
        Task<ServiceResult<ArticuloResponseDto>> Create(ArticuloRequestDto articulo);
        Task<ServiceResult<ArticuloResponseDto>> Update(int id, ArticuloRequestDto articulo);
        Task<ServiceResult<object>> Delete(int id);
    }
}
