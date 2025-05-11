namespace PracticaJWTcore.Dtos
{
    public class CreateAppoitmentDetailsDTO
    {
        public DateTime? AppointmentDate { get; set; }
        public string? Comments { get; set; }
        public long Vehicle { get; set; }
        public long Employee { get; set; }
        public List<CreateServicesDTODetails> Services { get; set; } = new List<CreateServicesDTODetails>();

    }
}
