﻿using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;

namespace PracticaJWTcore.Services

{
    public class AppoitmentServices : IAppointmentServices
    {
        private readonly IAppointmentRepository _appointmentRepository;
        public AppoitmentServices(IAppointmentRepository appointmentRepository) {
            _appointmentRepository = appointmentRepository;
        }
        public Task<long> CreateAppointment(Appointment appointment)
        {
            return _appointmentRepository.CreateAppointment(appointment);

        }

        public Task<bool> DeleteApointment(long id)
        {
            return _appointmentRepository.DeleteApointment(id);
        }

        public Task<IEnumerable<Appointment>> GetAppointment(long id)
        {
            return _appointmentRepository.GetAppointment(id);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentAll()
        {
            return await _appointmentRepository.GetAppointmentAll();
        }

 
    }
}
