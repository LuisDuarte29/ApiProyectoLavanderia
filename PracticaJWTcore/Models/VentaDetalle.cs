using System;

namespace PracticaJWTcore.Models
{
    // Entidad de VentaDetalles; cada registro representa un articulo vendido.
    public class VentaDetalle
    {
        public long IdVentaDetalle { get; set; }
        public long IdVenta { get; set; }
        public int IdArticulo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PorcentajeIva { get; set; }
        public decimal SubTotal { get; set; }
    }
}
