using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.DTOs;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public class EstadisticaRepository : IEstadisticaRepository
    {
        private readonly PracticaJWTcoreContext _context;
        public EstadisticaRepository(PracticaJWTcoreContext context)
        {
            _context = context;
        }


        public async Task<List<TipoLavadoDTO>> GetTipoLavado()
        {
            var tipoLavado = from t in _context.AppointmentServices join s in _context.Services on t.ServiceId equals s.ServiceId
                             group t by new { t.ServiceId, s.ServiceName } into g
                             select new TipoLavadoDTO
                             {
                                 IdService = g.Select(x => x.ServiceId).FirstOrDefault(),
                                 ServiceName = g.Select(x => x.Service.ServiceName).FirstOrDefault(),
                                 CantidadTotal = g.Count()
                             };

           return await tipoLavado.ToListAsync();
        }
    }
}
