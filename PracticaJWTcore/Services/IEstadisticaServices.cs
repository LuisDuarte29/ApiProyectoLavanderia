using PracticaJWTcore.DTOs;

namespace PracticaJWTcore.Services
{
    public interface IEstadisticaServices
    {
        Task<List<TipoLavadoDTO>> GetTipoLavado();
    }
}
