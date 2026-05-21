namespace PracticaJWTcore.Dtos
{
    // Request para crear appointment con cabecera y lista de servicios.
    public class CreateAppoitmentDetailsDTO
    {
        public DateTime? AppointmentDate { get; set; }
        public string? Comments { get; set; }
        public long Vehicle { get; set; }
        public long Employee { get; set; }
        public List<CreateServicesDTODetails> Services { get; set; } = new List<CreateServicesDTODetails>();

    }
}
