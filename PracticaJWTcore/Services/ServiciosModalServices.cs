using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;

namespace PracticaJWTcore.Services
{
    public class ServiciosModalServices : IServicioModal
    {
        public readonly PracticaJWTcoreContext _context;

        public ServiciosModalServices(PracticaJWTcoreContext context)
        {
            _context = context;
        }
        public async Task<List<ServiciosModal>> ServiciosModalsGetAll()
        {

            var serviciosModals = await _context.Services.Select(x => new ServiciosModal
            {

                ServiceId = x.ServiceId,
                ServiceName = x.ServiceName
            }).ToListAsync();
            return serviciosModals;
        }
    }
}
