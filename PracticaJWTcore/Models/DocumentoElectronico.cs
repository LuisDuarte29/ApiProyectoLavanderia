namespace PracticaJWTcore.Models
{
    public class DocumentoElectronico
    {
        public long IdDocumentoElectronico { get; set; }
        public long IdVenta { get; set; }
        public string TipoDocumento { get; set; } = null!;
        public string? NumeroDocumento { get; set; }
        public string EstadoFiscal { get; set; } = "PENDIENTE";
        public string? XmlContenido { get; set; }
        public string? XmlFirmado { get; set; }
        public string? CodigoRespuesta { get; set; }
        public string? MensajeRespuesta { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
