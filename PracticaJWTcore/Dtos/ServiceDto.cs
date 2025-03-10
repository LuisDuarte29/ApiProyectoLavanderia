namespace PracticaJWTcore.Dtos
{
    public class ServiceDto
    {
        public long ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public decimal? Price { get; set; }
        public string Estado { get; set; } // Estado de la tabla intermedia `AppointmentService`
    }
}