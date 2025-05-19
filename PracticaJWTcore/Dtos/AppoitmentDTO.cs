namespace PracticaJWTcore.Dtos
{
    public class AppoitmentDTO
    {
      
            public long AppointmentId { get; set; }
            
            public long? VehicleId { get; set; }

            public string VehicleString { get; set; }
            
            public long? EmployeeId { get; set; }

            public string EmployeeString { get; set; }

            public DateTime? AppointmentDate { get; set; }

            public string? Comments { get; set; }
        public List<long> ServiceId { get; set; }

    }
}
