using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticaJWTcore.Models;



public partial class Customer
{
    public long Id { get; set; }

    public string? FirstName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }



    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
