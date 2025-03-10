using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;

namespace PracticaJWTcore.Services
{
    public class PedidosServices : IPedidosServices
    {
        private readonly IPedidosRepository _pedidos;
        public PedidosServices(IPedidosRepository pedidos)
        {
            _pedidos = pedidos;
        }
        public async Task<List<AppoitmentDetailsDTO>> GetPedidos()
        {
            return await _pedidos.GetPedidos();
        }

    }
}
