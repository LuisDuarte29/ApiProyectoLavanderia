namespace PracticaJWTcore.Dtos.Ventas
{
    // Request de actualizacion parcial de venta; no recalcula detalles ni stock.
    public class UpdateVentaDto
    {
        public long? IdCliente { get; set; }
        public int? IdUsuario { get; set; }
        public string? MetodoPago { get; set; }
        public string? Estado { get; set; }
    }
}
