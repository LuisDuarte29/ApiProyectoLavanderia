namespace PracticaJWTcore.Dtos
{
    // Request para actualizar appointment y sincronizar sus servicios asociados.
    public class UpdateAppoitmentDetailsDTO
    {
        public long AppointmentId { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string? Comments { get; set; }
        public long Vehicle { get; set; }
        public long Employee { get; set; }
        public List<CreateServicesDTODetails> Services { get; set; } = new List<CreateServicesDTODetails>();
    }
}
