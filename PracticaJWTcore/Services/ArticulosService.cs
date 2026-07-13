using PracticaJWTcore.Dtos.Articulos;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;

namespace PracticaJWTcore.Services
{
    // Service de articulos: centraliza validaciones de catalogo antes de tocar EF Core.
    public class ArticulosService : IArticulosService
    {
        private readonly IArticulosRepository _repository;

        public ArticulosService(IArticulosRepository repository)
        {
            _repository = repository;
        }

        public Task<List<ArticuloResponseDto>> GetAll()
        {
            return _repository.GetAll();
        }

        public Task<ArticuloResponseDto?> GetById(int id)
        {
            return _repository.GetById(id);
        }

        public async Task<ServiceResult<ArticuloResponseDto>> Create(ArticuloRequestDto articulo)
        {
            // El alta valida datos de negocio y luego convierte el DTO en entidad persistible.
            var validation = await ValidateArticulo(articulo);
            if (!validation.Success)
                return ServiceResult<ArticuloResponseDto>.Fail(validation.Message!, validation.Code!);

            var entity = ToEntity(articulo);
            await _repository.Add(entity);
            await _repository.SaveChanges();

            return ServiceResult<ArticuloResponseDto>.Ok(ToResponse(entity));
        }

        public async Task<ServiceResult<ArticuloResponseDto>> Update(int id, ArticuloRequestDto articulo)
        {
            if (articulo == null)
                return ServiceResult<ArticuloResponseDto>.Fail("Articulo requerido desde ramaPrueba", "ARTICLE_REQUIRED");

            if (id != articulo.IdArticulo)
                return ServiceResult<ArticuloResponseDto>.Fail("El id de la ruta no coincide con el id del articulo", "ID_MISMATCH");

            var validation = await ValidateArticulo(articulo);
            if (!validation.Success)
                return ServiceResult<ArticuloResponseDto>.Fail(validation.Message!, validation.Code!);

            var existente = await _repository.GetEntityById(id);
            if (existente == null)
                return ServiceResult<ArticuloResponseDto>.Fail("Articulo no encontrado", "ARTICLE_NOT_FOUND");

            existente.NombreArticulo = articulo.NombreArticulo;
            existente.Precio = articulo.Precio;
            existente.Codigo = articulo.Codigo;
            existente.CodigoBarra = articulo.CodigoBarra;
            existente.Descripcion = articulo.Descripcion;
            existente.PrecioCosto = articulo.PrecioCosto;
            existente.PrecioVenta = articulo.PrecioVenta;
            existente.StockActual = articulo.StockActual;
            existente.StockMinimo = articulo.StockMinimo;
            existente.Activo = articulo.Activo;
            existente.IdCategoria = articulo.IdCategoria;

            await _repository.SaveChanges();
            return ServiceResult<ArticuloResponseDto>.Ok(ToResponse(existente));
        }

        public async Task<ServiceResult<object>> Delete(int id)
        {
            var articulo = await _repository.GetEntityById(id);
            if (articulo == null)
                return ServiceResult<object>.Fail("Articulo no encontrado", "ARTICLE_NOT_FOUND");

            // Bloquea eliminaciones que romperian trazabilidad de ventas o movimientos de stock.
            if (await _repository.HasVentasOrMovimientos(id))
                return ServiceResult<object>.Fail(
                    "No se puede eliminar el articulo porque tiene ventas o movimientos asociados",
                    "ARTICLE_HAS_DEPENDENCIES");

            _repository.Remove(articulo);
            await _repository.SaveChanges();
            return ServiceResult<object>.Ok(new object());
        }

        private async Task<ServiceResult<object>> ValidateArticulo(ArticuloRequestDto articulo)
        {
            // Estas validaciones protegen el catalogo sin depender del controller que llame al service.
            if (articulo == null)
                return ServiceResult<object>.Fail("Articulo requerido", "ARTICLE_REQUIRED");

            if (string.IsNullOrWhiteSpace(articulo.NombreArticulo))
                return ServiceResult<object>.Fail("NombreArticulo es requerido", "ARTICLE_NAME_REQUIRED");

            if (articulo.Precio.HasValue && articulo.Precio.Value < 0)
                return ServiceResult<object>.Fail("Precio no puede ser negativo", "INVALID_PRICE");

            if (articulo.PrecioCosto < 0 || articulo.PrecioVenta < 0)
                return ServiceResult<object>.Fail("Los precios no pueden ser negativos", "INVALID_PRICE");

            if (articulo.StockActual < 0 || articulo.StockMinimo < 0)
                return ServiceResult<object>.Fail("El stock no puede ser negativo", "INVALID_STOCK");

            if (articulo.IdCategoria.HasValue && !await _repository.CategoriaExists(articulo.IdCategoria.Value))
                return ServiceResult<object>.Fail($"Categoria {articulo.IdCategoria} no existe", "CATEGORY_NOT_FOUND");

            return ServiceResult<object>.Ok(new object());
        }

        private static Articulos ToEntity(ArticuloRequestDto articulo)
        {
            return new Articulos
            {
                IdArticulo = articulo.IdArticulo,
                NombreArticulo = articulo.NombreArticulo,
                Precio = articulo.Precio,
                Codigo = articulo.Codigo,
                CodigoBarra = articulo.CodigoBarra,
                Descripcion = articulo.Descripcion,
                PrecioCosto = articulo.PrecioCosto,
                PrecioVenta = articulo.PrecioVenta,
                StockActual = articulo.StockActual,
                StockMinimo = articulo.StockMinimo,
                Activo = articulo.Activo,
                IdCategoria = articulo.IdCategoria
            };
        }

        private static ArticuloResponseDto ToResponse(Articulos articulo)
        {
            return new ArticuloResponseDto
            {
                IdArticulo = articulo.IdArticulo,
                NombreArticulo = articulo.NombreArticulo,
                Precio = articulo.Precio,
                Codigo = articulo.Codigo,
                CodigoBarra = articulo.CodigoBarra,
                Descripcion = articulo.Descripcion,
                PrecioCosto = articulo.PrecioCosto,
                PrecioVenta = articulo.PrecioVenta,
                StockActual = articulo.StockActual,
                StockMinimo = articulo.StockMinimo,
                Activo = articulo.Activo,
                IdCategoria = articulo.IdCategoria
            };
        }
    }
}
