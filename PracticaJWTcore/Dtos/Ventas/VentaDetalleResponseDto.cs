namespace PracticaJWTcore.Dtos.Ventas
{
    // Response de cada detalle de venta, usado dentro de VentaResponseDto.
    public class VentaDetalleResponseDto
    {
        public long IdVentaDetalle { get; set; }
        public long IdVenta { get; set; }
        public int IdArticulo { get; set; }
        public string? NombreArticulo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PorcentajeIva { get; set; }
        public decimal SubTotal { get; set; }
    }
}
