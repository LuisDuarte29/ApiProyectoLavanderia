using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Services
{
    public interface IPedidosServices
    {
        Task<List<AppoitmentDetailsDTO>> GetPedidos();
    }
}
