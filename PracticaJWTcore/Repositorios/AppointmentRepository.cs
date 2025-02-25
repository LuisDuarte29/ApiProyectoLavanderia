using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly PracticaJWTcoreContext _context;
        public AppointmentRepository(PracticaJWTcoreContext context) {


            _context = context;
        }
        public async Task<long> CreateAppointment(Appointment appointments)
        {
            Appointment appointment = new Appointment()
            {
                AppointmentId=0,
                AppointmentDate=appointments.AppointmentDate,
                Employee=appointments.Employee,
                Vehicle=appointments.Vehicle,
                Comments=appointments.Comments
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment.AppointmentId;

        }

        public async Task<bool> DeleteApointment(long id)
        {
            var response=  await _context.Appointments.FirstAsync(x => x.AppointmentId == id);
           _context.Appointments.Remove(response);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Appointment>> GetAppointment(long id)
        {
            return (IEnumerable<Appointment>)await _context.Appointments.FirstAsync(x => x.AppointmentId == id);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentAll()
        {
            return await _context.Appointments.ToListAsync();
        }
    }
}
