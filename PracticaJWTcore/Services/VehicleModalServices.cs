using PracticaJWTcore.Repositorios;
using PracticaJWTcore.Models;
using Microsoft.EntityFrameworkCore;

namespace PracticaJWTcore.Services
{
    public class VehicleModalServices : IVehicleModal
    {
        private readonly PracticaJWTcoreContext _context;
        public VehicleModalServices(PracticaJWTcoreContext context)
        {
            _context = context;
        }
        public async Task<List<VehicleModal>> VehicleModalGetAll()
        {
            return await _context.Vehicles
                .Select(v => new VehicleModal
                {
                    IdVehicle = v.VehicleId,
                    VehicleName = v.Make
                }).ToListAsync();
        }
        
    }
}
