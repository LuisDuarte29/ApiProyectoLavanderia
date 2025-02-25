using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Services;

namespace PracticaJWTcore.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentServices _appointmentServices;
        public AppointmentController(IAppointmentServices appointmentServices)
        {
            _appointmentServices = appointmentServices;
        }
        public async Task<IActionResult> GetAppointmentAll()
        {
           var appointments = await _appointmentServices.GetAppointmentAll();
            return new OkObjectResult(appointments);
        }
    }
}
