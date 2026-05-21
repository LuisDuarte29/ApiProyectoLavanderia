using System.Text.Json.Serialization;

namespace PracticaJWTcore.Dtos.Stock
{
    // Response de movimientos de stock con datos proyectados para el frontend.
    public class StockMovimientoResponseDto
    {
        public long IdStockMovimiento { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public int IdArticulo { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? NombreArticulo { get; set; }

        public string TipoMovimiento { get; set; } = "SALIDA_VENTA";
        public decimal Cantidad { get; set; }
        public decimal? StockAnterior { get; set; }
        public decimal? StockNuevo { get; set; }
        public string? Referencia { get; set; }
        public string? Observacion { get; set; }
    }
}
