using PracticaJWTcore.Dtos.Stock;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;

namespace PracticaJWTcore.Services
{
    // Service de stock: valida movimientos manuales y mantiene el controller libre de reglas.
    public class StockService : IStockService
    {
        private readonly IStockRepository _repository;

        public StockService(IStockRepository repository)
        {
            _repository = repository;
        }

        public Task<List<StockMovimientoResponseDto>> GetMovimientos()
        {
            return _repository.GetMovimientos();
        }

        public Task<StockMovimientoResponseDto?> GetMovimiento(long id)
        {
            return _repository.GetMovimiento(id);
        }

        public async Task<ServiceResult<StockMovimientoResponseDto>> CreateMovimiento(StockMovimientoRequestDto movimiento)
        {
            // La creacion de movimientos valida articulo y cantidad antes de persistir.
            var validation = await ValidateMovimiento(movimiento);
            if (!validation.Success)
                return ServiceResult<StockMovimientoResponseDto>.Fail(validation.Message!, validation.Code!);

            return await _repository.ExecuteInTransaction(async () =>
            {
                var articulo = await _repository.GetArticulo(movimiento.IdArticulo);
                if (articulo == null)
                    return ServiceResult<StockMovimientoResponseDto>.Fail($"Articulo {movimiento.IdArticulo} no existe", "ARTICLE_NOT_FOUND");

                var tipoMovimiento = NormalizarTipoMovimiento(movimiento.TipoMovimiento);
                var stockAnterior = articulo.StockActual;

                if (tipoMovimiento == "Entrada")
                {
                    articulo.StockActual += movimiento.Cantidad;
                }
                else
                {
                    if (articulo.StockActual < movimiento.Cantidad)
                        return ServiceResult<StockMovimientoResponseDto>.Fail("Stock insuficiente para la salida solicitada", "STOCK_INSUFFICIENT");

                    articulo.StockActual -= movimiento.Cantidad;
                }

                var entity = new StockMovimiento
                {
                    IdStockMovimiento = 0,
                    FechaMovimiento = movimiento.FechaMovimiento == default ? DateTime.UtcNow : movimiento.FechaMovimiento,
                    IdArticulo = movimiento.IdArticulo,
                    TipoMovimiento = tipoMovimiento,
                    Cantidad = movimiento.Cantidad,
                    StockAnterior = stockAnterior,
                    StockNuevo = articulo.StockActual,
                    Referencia = movimiento.Referencia,
                    Observacion = movimiento.Observacion
                };

                await _repository.AddMovimiento(entity);
                await _repository.SaveChanges();

                return ServiceResult<StockMovimientoResponseDto>.Ok(ToResponse(entity));
            });
        }

        public async Task<ServiceResult<StockMovimientoResponseDto>> UpdateMovimiento(long id, StockMovimientoRequestDto movimiento)
        {
            if (movimiento == null)
                return ServiceResult<StockMovimientoResponseDto>.Fail("Movimiento requerido", "MOVEMENT_REQUIRED");

            if (id != movimiento.IdStockMovimiento)
                return ServiceResult<StockMovimientoResponseDto>.Fail("El id de la ruta no coincide con el id del movimiento", "ID_MISMATCH");

            var validation = await ValidateMovimiento(movimiento);
            if (!validation.Success)
                return ServiceResult<StockMovimientoResponseDto>.Fail(validation.Message!, validation.Code!);

            var existente = await _repository.GetMovimientoEntity(id);
            if (existente == null)
                return ServiceResult<StockMovimientoResponseDto>.Fail("Movimiento no encontrado", "MOVEMENT_NOT_FOUND");

            existente.FechaMovimiento = movimiento.FechaMovimiento == default ? existente.FechaMovimiento : movimiento.FechaMovimiento;
            existente.IdArticulo = movimiento.IdArticulo;
            existente.TipoMovimiento = movimiento.TipoMovimiento;
            existente.Cantidad = movimiento.Cantidad;
            existente.StockAnterior = movimiento.StockAnterior;
            existente.StockNuevo = movimiento.StockNuevo;
            existente.Referencia = movimiento.Referencia;
            existente.Observacion = movimiento.Observacion;

            await _repository.SaveChanges();
            return ServiceResult<StockMovimientoResponseDto>.Ok(ToResponse(existente));
        }

        public async Task<ServiceResult<object>> DeleteMovimiento(long id)
        {
            var movimiento = await _repository.GetMovimientoEntity(id);
            if (movimiento == null)
                return ServiceResult<object>.Fail("Movimiento no encontrado", "MOVEMENT_NOT_FOUND");

            _repository.RemoveMovimiento(movimiento);
            await _repository.SaveChanges();

            return ServiceResult<object>.Ok(new object());
        }

        private async Task<ServiceResult<object>> ValidateMovimiento(StockMovimientoRequestDto movimiento)
        {
            // Los movimientos manuales ajustan StockActual y registran el saldo anterior/nuevo en una transaccion.
            if (movimiento == null)
                return ServiceResult<object>.Fail("Movimiento requerido", "MOVEMENT_REQUIRED");

            if (movimiento.Cantidad <= 0)
                return ServiceResult<object>.Fail("La cantidad debe ser mayor a cero", "INVALID_QUANTITY");

            if (NormalizarTipoMovimiento(movimiento.TipoMovimiento) == string.Empty)
                return ServiceResult<object>.Fail("TipoMovimiento debe ser Entrada o Salida", "INVALID_MOVEMENT_TYPE");

            if (await _repository.GetArticulo(movimiento.IdArticulo) == null)
                return ServiceResult<object>.Fail($"Articulo {movimiento.IdArticulo} no existe", "ARTICLE_NOT_FOUND");

            return ServiceResult<object>.Ok(new object());
        }

        private static string NormalizarTipoMovimiento(string? tipoMovimiento)
        {
            var tipo = (tipoMovimiento ?? string.Empty).Trim().ToUpperInvariant();
            return tipo switch
            {
                "ENTRADA" or "INGRESO" or "COMPRA" => "Entrada",
                "SALIDA" or "EGRESO" or "SALIDA_VENTA" or "VENTA" => "Salida",
                _ => string.Empty
            };
        }

        private static StockMovimientoResponseDto ToResponse(StockMovimiento movimiento)
        {
            return new StockMovimientoResponseDto
            {
                IdStockMovimiento = movimiento.IdStockMovimiento,
                FechaMovimiento = movimiento.FechaMovimiento,
                IdArticulo = movimiento.IdArticulo,
                TipoMovimiento = movimiento.TipoMovimiento,
                Cantidad = movimiento.Cantidad,
                StockAnterior = movimiento.StockAnterior,
                StockNuevo = movimiento.StockNuevo,
                Referencia = movimiento.Referencia,
                Observacion = movimiento.Observacion
            };
        }
    }
}
