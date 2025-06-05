using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;
using System.Reflection.Metadata.Ecma335;

namespace PracticaJWTcore.Services
{
    public class ServiceServices : IServiceServices
    { 
        private readonly IServicesRepository _context;
        public ServiceServices(IServicesRepository context) {

            _context = context;
        }

        public async Task<Service> GetService(long id)
        {
            return await _context.GetService(id);
        }
        
        public async Task<Service> CreateService(Service services)
        {
            return await _context.CreateService(services);
        }
        public async Task<bool> DeleteService(long id)
        {
            return await _context.DeleteService(id);
        }
        public async Task<List<Service>> GetServiceAll()
        {
            return await _context.GetServiceAll();
        }

        public async Task<List<Service>> UpdateServices(Service services)
        {
            return await _context.UpdateServices(services);
        }

        public Task<List<Articulos>> GetArticulos()
        {
            return _context.GetAllArticulos();
        }
        
    }
}
