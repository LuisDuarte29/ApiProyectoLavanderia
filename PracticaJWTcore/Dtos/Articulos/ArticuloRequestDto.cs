namespace PracticaJWTcore.Dtos.Articulos
{
    // Request para alta/edicion de articulos; evita recibir la entidad EF directamente.
    public class ArticuloRequestDto
    {
        public int IdArticulo { get; set; }
        public string? NombreArticulo { get; set; }
        public decimal? Precio { get; set; }
        public string? Codigo { get; set; }
        public string? CodigoBarra { get; set; }
        public string? Descripcion { get; set; }
        public decimal PrecioCosto { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
        public bool Activo { get; set; }
        public int? IdCategoria { get; set; }
    }
}
