using PracticaJWTcore.Dtos.Ventas;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;

namespace PracticaJWTcore.Services
{
    // Service de ventas: concentra reglas de negocio para no mezclar stock, totales y HTTP.
    public class VentasService : IVentasService
    {
        private const decimal PorcentajeIvaDefault = 10.0m;
        private const string EstadoAnulada = "ANULADA";
        private readonly IVentasRepository _repository;

        public VentasService(IVentasRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<VentaResponseDto>> CreateVenta(CreateVentaDto dto)
        {
            // La venta solo puede avanzar si trae items validos y cantidades positivas.
            if (dto == null || dto.Items == null || !dto.Items.Any())
                return ServiceResult<VentaResponseDto>.Fail("Items requeridos", "ITEMS_REQUIRED");

            if (dto.Items.Any(i => i.Cantidad <= 0))
                return ServiceResult<VentaResponseDto>.Fail("La cantidad debe ser mayor a cero", "INVALID_QUANTITY");

            var articuloIds = dto.Items.Select(i => i.ArticuloId).Distinct().ToList();
            var articulos = await _repository.GetArticulosByIds(articuloIds);

            // Valida existencia y stock antes de abrir la transaccion de escritura.
            foreach (var item in dto.Items)
            {
                var articulo = articulos.FirstOrDefault(a => a.IdArticulo == item.ArticuloId);
                if (articulo == null)
                    return ServiceResult<VentaResponseDto>.Fail($"Articulo {item.ArticuloId} no existe", "ARTICLE_NOT_FOUND");

                if (articulo.StockActual < item.Cantidad)
                    return ServiceResult<VentaResponseDto>.Fail(
                        $"Stock insuficiente para articulo {articulo.IdArticulo}. Disponible: {articulo.StockActual}",
                        "STOCK_INSUFFICIENT");
            }

            return await _repository.ExecuteInTransaction(async () =>
            {
                // La transaccion coordina venta, detalles, descuento de stock y movimientos como una unidad.
                var venta = new Venta
                {
                    IdCliente = dto.IdCliente,
                    IdUsuario = dto.IdUsuario,
                    FechaVenta = DateTime.UtcNow,
                    MetodoPago = dto.MetodoPago,
                    Estado = string.IsNullOrWhiteSpace(dto.Estado) ? "CONFIRMADA" : dto.Estado
                };

                await _repository.AddVenta(venta);
                await _repository.SaveChanges();

                decimal subTotal = 0m;
                decimal total = 0m;

                foreach (var item in dto.Items)
                {
                    var articulo = articulos.First(a => a.IdArticulo == item.ArticuloId);
                    var precio = articulo.PrecioVenta != 0 ? articulo.PrecioVenta : (articulo.Precio ?? 0);
                    var subtotalLinea = precio * item.Cantidad;
                    var ivaLinea = subtotalLinea * (PorcentajeIvaDefault / 100m);

                    // Cada item queda registrado como detalle para conservar el historial de la venta.
                    await _repository.AddVentaDetalle(new VentaDetalle
                    {
                        IdVenta = venta.IdVenta,
                        IdArticulo = articulo.IdArticulo,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = precio,
                        PorcentajeIva = PorcentajeIvaDefault,
                        SubTotal = subtotalLinea
                    });

                    var stockAnterior = articulo.StockActual;
                    articulo.StockActual -= item.Cantidad;

                    // El movimiento deja trazabilidad de la salida de stock generada por la venta.
                    await _repository.AddStockMovimiento(new StockMovimiento
                    {
                        IdArticulo = articulo.IdArticulo,
                        Cantidad = item.Cantidad,
                        FechaMovimiento = DateTime.UtcNow,
                        TipoMovimiento = "Salida",
                        StockAnterior = stockAnterior,
                        StockNuevo = articulo.StockActual,
                        Referencia = $"Venta:{venta.IdVenta}"
                    });

                    subTotal += subtotalLinea;
                    total += subtotalLinea + ivaLinea;
                }

                venta.SubTotal = subTotal;
                venta.IvaTotal = total - subTotal;
                venta.Total = total;

                await _repository.SaveChanges();

                var response = await _repository.GetVentaResponseById(venta.IdVenta);
                return ServiceResult<VentaResponseDto>.Ok(response!);
            });
        }

        public Task<List<VentaResponseDto>> GetVentas()
        {
            return _repository.GetVentas();
        }

        public Task<VentaResponseDto?> GetVenta(long id)
        {
            return GetVentaInternal(id);
        }

        private async Task<VentaResponseDto?> GetVentaInternal(long id)
        {
            var ventas = await _repository.GetVentas();
            return ventas.FirstOrDefault(v => v.IdVenta == id);
        }

        public async Task<ServiceResult<object>> UpdateVenta(long id, UpdateVentaDto dto)
        {
            // Update valida referencias basicas sin recalcular detalles ni stock.
            if (dto == null)
                return ServiceResult<object>.Fail("Venta requerida", "VENTA_REQUIRED");

            var venta = await _repository.GetVentaById(id);
            if (venta == null)
                return ServiceResult<object>.Fail("Venta no encontrada", "VENTA_NOT_FOUND");

            if (dto.IdCliente.HasValue && !await _repository.ExistsCliente(dto.IdCliente.Value))
                return ServiceResult<object>.Fail($"Cliente {dto.IdCliente} no existe", "CLIENT_NOT_FOUND");

            if (dto.IdUsuario.HasValue && !await _repository.ExistsUsuario(dto.IdUsuario.Value))
                return ServiceResult<object>.Fail($"Usuario {dto.IdUsuario} no existe", "USER_NOT_FOUND");

            venta.IdCliente = dto.IdCliente;
            venta.IdUsuario = dto.IdUsuario;
            venta.MetodoPago = dto.MetodoPago;
            if (!string.IsNullOrWhiteSpace(dto.Estado))
                venta.Estado = dto.Estado;

            await _repository.SaveChanges();

            return ServiceResult<object>.Ok(venta);
        }

        public async Task<ServiceResult<object>> DeleteVenta(long id)
        {
            var venta = await _repository.GetVentaWithDetalles(id);
            if (venta == null)
                return ServiceResult<object>.Fail("Venta no encontrada", "VENTA_NOT_FOUND");

            if (venta.Estado == EstadoAnulada)
                return ServiceResult<object>.Fail("La venta ya esta anulada", "VENTA_ALREADY_CANCELLED");

            return await _repository.ExecuteInTransaction(async () =>
            {
                // Al eliminar una venta se restaura el stock de sus detalles y se registra el ajuste.
                var articuloIds = venta.VentaDetalles.Select(d => d.IdArticulo).Distinct();
                var articulos = await _repository.GetArticulosByIds(articuloIds);

                foreach (var detalle in venta.VentaDetalles)
                {
                    var articulo = articulos.FirstOrDefault(a => a.IdArticulo == detalle.IdArticulo);
                    if (articulo == null)
                        continue;

                    var stockAnterior = articulo.StockActual;
                    articulo.StockActual += detalle.Cantidad;

                    await _repository.AddStockMovimiento(new StockMovimiento
                    {
                        IdArticulo = articulo.IdArticulo,
                        FechaMovimiento = DateTime.UtcNow,
                        TipoMovimiento = "Entrada",
                        Cantidad = detalle.Cantidad,
                        StockAnterior = stockAnterior,
                        StockNuevo = articulo.StockActual,
                        Referencia = $"AnulacionVenta:{venta.IdVenta}",
                        Observacion = "Stock restaurado por anulacion de venta"
                    });
                }

                venta.Estado = EstadoAnulada;
                venta.FechaAnulacion = DateTime.UtcNow;
                venta.MotivoAnulacion = "Anulacion desde API";

                await _repository.SaveChanges();
                return ServiceResult<object>.Ok(new object());
            });
        }
    }
}
