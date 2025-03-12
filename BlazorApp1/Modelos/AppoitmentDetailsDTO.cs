

namespace BlazorApp1.Modelos
{
    public class AppoitmentDetailsDTO
    {
        public long AppointmentId { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string? Comments { get; set; }
        public string? Vehicle { get; set; }
        public string? Employee { get; set; }
        public List<ServiceDTO> Services { get; set; } = new List<ServiceDTO>();
    }
}
