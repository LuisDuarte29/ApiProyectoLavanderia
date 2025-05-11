using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Models;
using PracticaJWTcore.Dtos;

namespace PracticaJWTcore.Repositorios
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly PracticaJWTcoreContext _context;
  
        public AppointmentRepository(PracticaJWTcoreContext context) {


            _context = context;
        }
        public async Task<long> CreateAppointment(CreateAppoitmentDetailsDTO appointments)
        {
            Appointment appointment = new Appointment()
            {
             
                AppointmentDate=appointments.AppointmentDate,
                EmployeeId =appointments.Employee,
                VehicleId=appointments.Vehicle,
                Comments=appointments.Comments
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

          var servicesDetails=appointments.Services.Select(s => new AppointmentService
            {
                AppointmentId = appointment.AppointmentId,
                ServiceId = s.ServiceId,
                Estado = "Pendiente"
            });
            await _context.AppointmentServices.AddRangeAsync(servicesDetails);
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

        public async Task<AppoitmentDTO?> GetAppointment(long id)
        {
            return await _context.Appointments.Where(m=>m.AppointmentId==id).Select(x => new AppoitmentDTO
            {
                AppointmentId = x.AppointmentId,
                AppointmentDate = x.AppointmentDate,
                EmployeeString = _context.Customer
               .Where(e => e.Id == x.EmployeeId)
               .Select(e => e.FirstName)
               .FirstOrDefault() ?? string.Empty, // Fix for CS8601
                Comments = x.Comments,
                VehicleString = _context.Vehicles
               .Where(e => e.VehicleId == x.VehicleId)
               .Select(e => e.OwnerName)
               .FirstOrDefault() ?? string.Empty // Fix for CS8601
            }).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AppoitmentDTO>> GetAppointmentAll()
        {
            return await _context.Appointments.Select(x => new AppoitmentDTO
            {
                AppointmentId = x.AppointmentId,
                AppointmentDate = x.AppointmentDate,
                EmployeeString = _context.Customer
                    .Where(e => e.Id == x.EmployeeId)
                    .Select(e => e.FirstName)
                    .FirstOrDefault() ?? string.Empty, // Fix for CS8601
                Comments = x.Comments,
                VehicleString = _context.Vehicles
                    .Where(e => e.VehicleId == x.VehicleId)
                    .Select(e => e.OwnerName)
                    .FirstOrDefault() ?? string.Empty // Fix for CS8601
            }).ToListAsync();
        }

        public async Task<IEnumerable<AppoitmentDTO>> UpdateAppointment(Appointment appointment)
        {
            Appointment appointment1 = await _context.Appointments.FirstAsync(x => x.AppointmentId == appointment.AppointmentId);

            appointment1.AppointmentDate = appointment.AppointmentDate;
            appointment1.Employee = appointment.Employee;
            appointment1.Vehicle = appointment.Vehicle;
            appointment1.Comments = appointment.Comments;
            await _context.SaveChangesAsync();
            return await GetAppointmentAll();
        }
    }
}
