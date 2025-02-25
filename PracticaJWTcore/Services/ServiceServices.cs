using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Services
{
    public class ServiceServices
    { 
        private readonly PracticaJWTcoreContext _context;
        public ServiceServices(PracticaJWTcoreContext context) {

            _context = context;
        }

      

        public async Task<Service> GetService(long id)
        {
            return await _context.Services.FirstAsync(x => x.ServiceId == id);
        }
        
        public async Task<Service> CreateService(Service services)
        {
            Service servicesEntity = new Service
            {
                ServiceId = 0,
                ServiceName = services.ServiceName,
                Description = services.Description,
                Price = services.Price
            };
            EntityEntry<Service> entityEntry=await _context.Services.AddAsync(servicesEntity);
            await _context.SaveChangesAsync();
            return await GetService(entityEntry.Entity.ServiceId);
        }


    }
}
