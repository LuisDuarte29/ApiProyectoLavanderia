namespace PracticaJWTcore.Dtos.Ventas
{
    // Item de venta recibido desde el frontend; el service valida existencia y stock.
    public class VentaItemDto
    {
        public int ArticuloId { get; set; }
        public decimal Cantidad { get; set; }
    }
}
