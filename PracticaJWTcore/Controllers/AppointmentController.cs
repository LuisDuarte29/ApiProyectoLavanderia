using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Models;
using PracticaJWTcore.Services;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles ="Admin")]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentServices _appointmentServices;
        public AppointmentController(IAppointmentServices appointmentServices)
        {
            _appointmentServices = appointmentServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointmentAll()
        {
            var appointments = await _appointmentServices.GetAppointmentAll();
            return new OkObjectResult(appointments);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointment(long id)
        {
            var appointment = await _appointmentServices.GetAppointment(id);
            return new OkObjectResult(appointment);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(long id)
        {
            var response = await _appointmentServices.DeleteApointment(id);
            return response ? Ok() : NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] Appointment appointment)
        {
            var response = await _appointmentServices.CreateAppointment(appointment);
            return new CreatedResult($"https://localhost:7184/api/Appointment/{appointment.AppointmentId}", null);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAppointment([FromBody] Appointment appointment)
        {
            var response= await _appointmentServices.UpdateAppointment(appointment);
            return new OkObjectResult(response);
        }
    }
}
