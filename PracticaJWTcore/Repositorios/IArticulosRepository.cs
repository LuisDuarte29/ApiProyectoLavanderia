using PracticaJWTcore.Dtos.Articulos;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public interface IArticulosRepository
    {
        Task<List<ArticuloResponseDto>> GetAll();
        Task<ArticuloResponseDto?> GetById(int id);
        Task<Articulos?> GetEntityById(int id);
        Task<bool> CategoriaExists(int idCategoria);
        Task<bool> HasVentasOrMovimientos(int idArticulo);
        Task Add(Articulos articulo);
        void Remove(Articulos articulo);
        Task SaveChanges();
    }
}
