namespace PracticaJWTcore.Dtos.Stock
{
    // Request para movimientos manuales de stock; el service valida articulo y cantidad.
    public class StockMovimientoRequestDto
    {
        public long IdStockMovimiento { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public int IdArticulo { get; set; }
        public string TipoMovimiento { get; set; } = "SALIDA_VENTA";
        public decimal Cantidad { get; set; }
        public decimal? StockAnterior { get; set; }
        public decimal? StockNuevo { get; set; }
        public string? Referencia { get; set; }
        public string? Observacion { get; set; }
    }
}
