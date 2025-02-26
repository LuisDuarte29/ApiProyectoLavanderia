using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAppointmentAll();
        Task<Appointment> GetAppointment(long id);
        Task<bool> DeleteApointment(long id);
        Task<long> CreateAppointment(Appointment appointment);
        Task<IEnumerable<Appointment>> UpdateAppointment(Appointment appointment);
}
    }
