﻿using System.Text.Json.Serialization;

namespace BlazorApp1.Modelos
{
    public partial class Service
    {
        public long ServiceId { get; set; }

        public string? ServiceName { get; set; }

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        [JsonIgnore]
        public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
    }
}
