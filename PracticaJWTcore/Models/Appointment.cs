using System;
using System.Collections.Generic;

namespace PracticaJWTcore.Models;

public partial class Appointment
{
    public long AppointmentId { get; set; }

    public long? VehicleId { get; set; }

    public long? EmployeeId { get; set; }

    public DateTime? AppointmentDate { get; set; }

    public string? Comments { get; set; }

    public virtual Customer? Employee { get; set; }

    public virtual Vehicle? Vehicle { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
