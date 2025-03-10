using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;

namespace PracticaJWTcore.Repositorios
{
    public class PedidosRepository : IPedidosRepository
    {
        private readonly PracticaJWTcoreContext _context;
        public PedidosRepository(PracticaJWTcoreContext context)
        {
            _context = context;
        }
        public async Task<List<AppoitmentDetailsDTO>> GetPedidos()
        {

            var appointmentDetails = await _context.Appointments
       
               .Select(a => new AppoitmentDetailsDTO
               {
                   AppointmentId = a.AppointmentId,
                   AppointmentDate = a.AppointmentDate,
                   Comments = a.Comments,
                   Vehicle = a.Vehicle != null ? a.Vehicle.Make.ToString() : "Sin vehículo",
                   Employee = a.Employee != null ? a.Employee.FirstName.ToString() : "Sin empleado",
                   Services = a.AppointmentServices.Select(x => new ServiceDto
                   {
                       ServiceId = x.Service.ServiceId,
                       ServiceName = x.Service.ServiceName,
                       Price = x.Service.Price,
                       Estado = x.Estado // Estado de la tabla `AppointmentService`
                   }).ToList()
               }).ToListAsync();
            return appointmentDetails;

        }
    }
}
