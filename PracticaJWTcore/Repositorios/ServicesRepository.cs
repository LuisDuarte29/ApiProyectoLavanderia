using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public class ServicesRepository: IServicesRepository
    {

        private readonly PracticaJWTcoreContext _context;
        public ServicesRepository(PracticaJWTcoreContext context)
        {

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
            EntityEntry<Service> entityEntry = await _context.Services.AddAsync(servicesEntity);
            await _context.SaveChangesAsync();
            return await GetService(entityEntry.Entity.ServiceId);
        }
        public async Task<bool> DeleteService(long id)
        {
            Service services = await GetService(id);
            _context.Services.Remove(services);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<Service>> GetServiceAll()
        {
            return await _context.Services.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Service>> UpdateServices(Service services)
        {
            Service servicesEntity = await _context.Services.FirstAsync(x => x.ServiceId == services.ServiceId);
            servicesEntity.ServiceName = services.ServiceName;
            servicesEntity.Description = services.Description;
            servicesEntity.Price = services.Price;
            await _context.SaveChangesAsync();
            return await GetServiceAll();
        }

        public async Task<IEnumerable<Articulos>> GetAllArticulos()
        {
            return await _context.Articulos.ToListAsync();
        }
    }
}

