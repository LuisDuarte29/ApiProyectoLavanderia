namespace PracticaJWTcore.Dtos
{
    public class AppoitmentDTO
    {
      
            public long AppointmentId { get; set; }

            public string VehicleString { get; set; }

            public string EmployeeString { get; set; }

            public DateTime? AppointmentDate { get; set; }

            public string? Comments { get; set; }

    }
}
