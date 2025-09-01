using PracticaJWTcore.DTOs;
using PracticaJWTcore.Repositorios;

namespace PracticaJWTcore.Services
{
    public class EstadisiticaServices : IEstadisticaServices
    {
        public readonly IEstadisticaRepository _repository;

        public EstadisiticaServices(IEstadisticaRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task<List<TipoLavadoDTO>> GetTipoLavado()
        {
           return await _repository.GetTipoLavado();
        }
    }
}
