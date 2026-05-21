using System;

namespace PracticaJWTcore.Models
{
    // Entidad de StockMovimientos; deja trazabilidad de entradas/salidas o ajustes.
    public class StockMovimiento
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
