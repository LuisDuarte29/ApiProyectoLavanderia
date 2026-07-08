namespace PracticaJWTcore.Dtos.Dashboard
{
    // Response compacto para cards principales del dashboard de ventas.
    public class DashboardResumenVentasDto
    {
        public decimal TotalVentasDia { get; set; }
        public decimal TotalVentasMes { get; set; }
        public int CantidadVentasDia { get; set; }
        public int CantidadVentasMes { get; set; }
        public decimal CantidadArticulosVendidosDia { get; set; }
        public decimal CantidadArticulosVendidosMes { get; set; }
        public string? ProductoMasVendido { get; set; }
    }
}
