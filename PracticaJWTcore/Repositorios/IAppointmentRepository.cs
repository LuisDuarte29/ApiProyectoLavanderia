using PracticaJWTcore.Models;
using PracticaJWTcore.Dtos;

namespace PracticaJWTcore.Repositorios
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<AppoitmentDTO>> GetAppointmentAll();
        Task<AppoitmentDTO> GetAppointment(long id);
        Task<bool> DeleteApointment(long id);
        Task<long> CreateAppointment(CreateAppoitmentDetailsDTO appointment);
        Task<IEnumerable<AppoitmentDTO>> UpdateAppointment(Appointment appointment);
}
    }
