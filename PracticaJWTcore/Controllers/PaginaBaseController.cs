using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;
using PracticaJWTcore.Services;


namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaginaBaseController : Controller
    {
        public readonly IVehicleModal _vehicleModal;
        public readonly ICustomerModal _customerModal;
        public readonly IServicioModal _servicioModal;

        public PaginaBaseController(IVehicleModal vehicleModal, ICustomerModal customerModal, IServicioModal servicioModal)
        {
            _vehicleModal = vehicleModal;
            _customerModal = customerModal;
            _servicioModal = servicioModal;
        }

        [HttpGet("vehicle")]
        public async Task<IActionResult> VehicleModalGetAll()
        {
            var vehicleModalsList = await _vehicleModal.VehicleModalGetAll();
            return new OkObjectResult(vehicleModalsList);
        }
        [HttpGet("customer")]
        public async Task<IActionResult> CustomerModalGetAll()
        {
            var customerModalsList = await _customerModal.CustomerModalGetAll();
            return new OkObjectResult(customerModalsList);
        }
        [HttpGet("servicios")]

        public async Task<IActionResult> ServiciosModalGetAll()
        {
            var serviciosModalsList = await _servicioModal.ServiciosModalsGetAll();
            return new OkObjectResult(serviciosModalsList);
        }

    }
}
