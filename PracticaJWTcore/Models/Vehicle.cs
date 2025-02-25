using System;
using System.Collections.Generic;

namespace PracticaJWTcore.Models;

public partial class Vehicle
{
    public long VehicleId { get; set; }

    public string? LicensePlate { get; set; }

    public string? Make { get; set; }

    public string? Model { get; set; }

    public string? OwnerName { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
