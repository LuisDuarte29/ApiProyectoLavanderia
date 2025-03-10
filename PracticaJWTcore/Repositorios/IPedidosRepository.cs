using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public interface IPedidosRepository
    {
        Task<List<AppoitmentDetailsDTO>> GetPedidos();

    }
}
