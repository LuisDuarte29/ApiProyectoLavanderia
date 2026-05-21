using PracticaJWTcore.Entities;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PracticaJWTcore.Models;

// Entidad de Appointment; cabecera de pedido/turno con vehiculo, empleado y servicios.
public partial class Appointment
{
    public long AppointmentId { get; set; }

    public long? VehicleId { get; set; }

    public long EmployeeId { get; set; }

    public DateTime? AppointmentDate { get; set; }

    public string? Comments { get; set; }

    public virtual Customer? Employee { get; set; }

    public virtual Vehicle? Vehicle { get; set; }

    [JsonIgnore]
    public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
}
