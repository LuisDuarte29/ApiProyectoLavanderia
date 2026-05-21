using PracticaJWTcore.DTOs;

namespace PracticaJWTcore.Repositorios
{
    public interface IEstadisticaRepository
    {
        Task<List<TipoLavadoDTO>> GetTipoLavado();
    }
}
