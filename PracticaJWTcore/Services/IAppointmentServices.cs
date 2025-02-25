using PracticaJWTcore.Models;

namespace PracticaJWTcore.Services
{
    public interface IAppointmentServices
    {
      Task<IEnumerable<Appointment>> GetAppointmentAll();
        Task<IEnumerable<Appointment>> GetAppointment(long id);
        Task<bool> DeleteApointment(long id);
        Task<long> CreateAppointment(Appointment appointment);
    }
}
