using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Models;
using PracticaJWTcore.Dtos;
using PracticaJWTcore.Services;

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
             _context.AppointmentServices.AddRange(servicesDetails);
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
                VehicleId=x.VehicleId,
                EmployeeId=x.EmployeeId,
                ServiceId=_context.AppointmentServices.Where(e=>e.AppointmentId==x.AppointmentId).Select(e => e.ServiceId).ToList(),
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
            return await _context.Appointments.AsNoTracking().Select(x => new AppoitmentDTO
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

        public async Task<IEnumerable<AppoitmentDTO>> UpdateAppointment(UpdateAppoitmentDetailsDTO appointment)
        {
            //Verificar si el appoitment existe y me trae el registro y sus servicios
            Appointment appointment1 = await _context.Appointments.Include(a=>a.AppointmentServices).FirstAsync(a => a.AppointmentId == appointment.AppointmentId);

            //Relleno o actualizo los campos de cabecera
            appointment1.AppointmentDate = appointment.AppointmentDate;
            appointment1.EmployeeId = appointment.Employee;
            appointment1.VehicleId = appointment.Vehicle;
            appointment1.Comments = appointment.Comments;

            //Enlisto los servicios que vienen del front y los que ya existen en la base de datos
            var inServicesIdUpdate = appointment.Services.Select(y => y.ServiceId).ToList();
            var existingId = appointment1.AppointmentServices.Select(s => s.ServiceId).ToList();


            //Verifico los servicios que se agregan y los que se eliminan con el except donde el primero es el que se queda 
            //y el segundo es el que se elimina
            var servicesToAdd = inServicesIdUpdate.Except(existingId).ToList();
            var servicesToRemove=existingId.Except(inServicesIdUpdate).ToList();

            //Agrego los servicios nuevos en la base de datos
            foreach (var item in servicesToAdd)
            {
                var appointmentService = new AppointmentService
                {
                    AppointmentId = appointment.AppointmentId,
                    ServiceId = item,
                    Estado = "Pendiente" // Asumiendo que agregaste este campo
                };

                _context.AppointmentServices.Add(appointmentService);
            }
            

            //Elimino los servicios que ya no existen en la base de datos
            foreach(var item in servicesToRemove)
            {
                var appoitmentService = await _context.AppointmentServices.FirstOrDefaultAsync(x => x.ServiceId == item && x.AppointmentId==appointment.AppointmentId );
            
                if (appoitmentService != null)
                {
                    _context.AppointmentServices.Remove(appoitmentService);
                }
            }



            await _context.SaveChangesAsync();
            return await GetAppointmentAll();
        }
    }
}
