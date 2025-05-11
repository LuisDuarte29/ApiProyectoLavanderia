using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Services
{
    public interface IAppointmentServices
    {
      Task<IEnumerable<AppoitmentDTO>> GetAppointmentAll();
        Task<AppoitmentDTO> GetAppointment(long id);
        Task<bool> DeleteApointment(long id);
        Task<long> CreateAppointment(CreateAppoitmentDetailsDTO appointment);
        Task<IEnumerable<AppoitmentDTO>> UpdateAppointment(Appointment appointment);
    }
}
