namespace PracticaJWTcore.Dtos.Dashboard
{
    // Response para alertas de articulos con stock actual en o por debajo del minimo.
    public class DashboardStockBajoDto
    {
        public int IdArticulo { get; set; }
        public string? NombreArticulo { get; set; }
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
    }
}
