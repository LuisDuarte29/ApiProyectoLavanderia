namespace PracticaJWTcore.Dtos.Dashboard
{
    // Response de ranking de productos vendidos, incluyendo stock actual disponible.
    public class DashboardProductoMasVendidoDto
    {
        public int IdArticulo { get; set; }
        public string? NombreArticulo { get; set; }
        public decimal CantidadVendida { get; set; }
        public decimal TotalVendido { get; set; }
        public decimal? StockActual { get; set; }
    }
}
