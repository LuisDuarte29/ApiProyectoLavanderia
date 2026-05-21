namespace PracticaJWTcore.Dtos.Ventas
{
    // Request usado por POST /api/Ventas: cabecera de venta mas items a vender.
    public class CreateVentaDto
    {
        public long? IdCliente { get; set; }
        public int? IdUsuario { get; set; }
        public string? MetodoPago { get; set; }
        public string? Estado { get; set; }
        public List<VentaItemDto> Items { get; set; } = new List<VentaItemDto>();
    }
}
