using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;

namespace PracticaJWTcore.Services

{
    // Service de appointments: coordina el controller con el repository que maneja cabecera y servicios.
    public class AppoitmentServices : IAppointmentServices
    {
        private readonly IAppointmentRepository _appointmentRepository;
        public AppoitmentServices(IAppointmentRepository appointmentRepository) {
            _appointmentRepository = appointmentRepository;
        }
        public Task<long> CreateAppointment(CreateAppoitmentDetailsDTO appointment)
        {
            return _appointmentRepository.CreateAppointment(appointment);

        }
        public Task<bool> DeleteApointment(long id)
        {
            return _appointmentRepository.DeleteApointment(id);
        }

        public Task<AppoitmentDTO> GetAppointment(long id)
        {
            return _appointmentRepository.GetAppointment(id);
        }

        public async Task<IEnumerable<AppoitmentDTO>> GetAppointmentAll()
        {
            return await _appointmentRepository.GetAppointmentAll();
        }

        public async Task<IEnumerable<AppoitmentDTO>> UpdateAppointment(UpdateAppoitmentDetailsDTO appointment)
        {
            return await _appointmentRepository.UpdateAppointment(appointment);
            
        }

  
    }
}
