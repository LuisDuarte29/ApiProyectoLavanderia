using PracticaJWTcore.Dtos;
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

        public async Task<int> CambioClave(CambioClave cambio)
        {
            return await _context.CambioClave(cambio);
        }

        public async Task<TokenRolDTO> Login(UsuarioLogin usuario)
        {
            return await _context.Login(usuario);
        }
    }
}
