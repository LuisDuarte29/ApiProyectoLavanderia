using System.Text.Json.Serialization;

namespace PracticaJWTcore.Dtos.Articulos
{
    // Response de articulos; agrega informacion de categoria sin exponer navegaciones EF.
    public class ArticuloResponseDto
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

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? NombreCategoria { get; set; }
    }
}
