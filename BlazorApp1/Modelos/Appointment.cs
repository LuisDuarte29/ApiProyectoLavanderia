using PracticaJWTcore.Models;
using System.Text.Json.Serialization;

namespace BlazorApp1.Modelos
{
    public partial class Appointment
    {
        public long AppointmentId { get; set; }

        public long? VehicleId { get; set; }

        public long? EmployeeId { get; set; }

        public DateTime? AppointmentDate { get; set; }

        public string? Comments { get; set; }

        public virtual Customer? Employee { get; set; }

        public virtual Vehicle? Vehicle { get; set; }

        [JsonIgnore]
        public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
    }
}
