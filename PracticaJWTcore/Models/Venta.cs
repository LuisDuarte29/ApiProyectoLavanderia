using System;
using System.Collections.Generic;

namespace PracticaJWTcore.Models
{
    // Entidad de la tabla Ventas; representa la cabecera y totales de una venta.
    public class Venta
    {
        public long IdVenta { get; set; }
        public DateTime FechaVenta { get; set; } = DateTime.UtcNow;
        public long? IdCliente { get; set; }
        public int? IdUsuario { get; set; }
        public decimal SubTotal { get; set; }
        public decimal IvaTotal { get; set; }
        public decimal Total { get; set; }
        public string? MetodoPago { get; set; }
        public string Estado { get; set; } = "CONFIRMADA";
        public DateTime? FechaAnulacion { get; set; }
        public string? MotivoAnulacion { get; set; }
        public int? IdUsuarioAnulacion { get; set; }

        public virtual List<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
    }
}
