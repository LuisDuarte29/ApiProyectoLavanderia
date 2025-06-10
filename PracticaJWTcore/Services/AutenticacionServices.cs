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

        public async Task<int> CambioClave(string nuevaClave, string correo)
        {
            return await _context.CambioClave(nuevaClave, correo);
        }

        public async Task<string> Login(UsuarioLogin usuario)
        {
            return await _context.Login(usuario);
        }
    }
}
