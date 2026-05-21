using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PracticaJWTcore.Models;

// Entidad de Services; actualmente tambien se usa como contrato en endpoints historicos.
public partial class Service
{
    public long ServiceId { get; set; }

    public string? ServiceName { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    [JsonIgnore]
    public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
}
