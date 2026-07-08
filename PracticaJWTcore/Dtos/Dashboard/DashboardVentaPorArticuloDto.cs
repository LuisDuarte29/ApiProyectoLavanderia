namespace PracticaJWTcore.Dtos.Dashboard
{
    // Response agrupado por articulo para graficos de ventas.
    public class DashboardVentaPorArticuloDto
    {
        public int IdArticulo { get; set; }
        public string? NombreArticulo { get; set; }
        public decimal CantidadVendida { get; set; }
        public decimal TotalVendido { get; set; }
    }
}
