using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;

namespace PracticaJWTcore.Services
{
    public class AutenticacionServices:IAutenticacionServices
    {
        private readonly IAutenticacionRepository _context;
        public AutenticacionServices(IAutenticacionRepository context)
        {
            _context = context;
        }

        public async Task<string> Login(Usuario usuario)
        {
            return await _context.Login(usuario);
        }
    }
}
