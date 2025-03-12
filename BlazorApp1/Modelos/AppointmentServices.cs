namespace BlazorApp1.Modelos
{
    public partial class AppointmentService
    {
        public long AppointmentId { get; set; }

        public long ServiceId { get; set; }

        //Al hacer scaffold si o si debemos poner un id a la tabla en la base de datos como un primary key ya que asi 
        //va a generarnos la tabla terciaria que necesitamos 
        public long IdAppointmentServices { get; set; }
        public string Estado { get; set; }
        public virtual Appointment Appointment { get; set; } = null!;
        public virtual Service Service { get; set; } = null!;
    }

}
