using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Models;
using PracticaJWTcore.Services;
using System.Runtime.CompilerServices;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    // Controller de servicios: conserva endpoints historicos que aun usan la entidad Service como contrato.
    public class ServiceController : Controller
    {
        private readonly IServiceServices _serviceServices;
        public ServiceController(IServiceServices servicesServices)
        {
            _serviceServices = servicesServices;
        }

        [Authorize]
        [HttpGet("{idServicioLong}")]
        public async Task<IActionResult> GetService(long idServicioLong)
        {
            var service = await _serviceServices.GetService(idServicioLong);
            return new OkObjectResult(service);
        }
        [Authorize]
        //En el post si o si debo enviarle igual el id del servicio = 0, no aceptan null
        [HttpPost]
        // El alta delega persistencia al service y devuelve la Location esperada por las pruebas/consumidores.
        public async Task<IActionResult> CreateServices([FromBody] Service ServicesBody)
        {
            var service = await _serviceServices.CreateService(ServicesBody);
            return new CreatedResult($"api/Service/{service.ServiceId}", null);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(long id)
        {
            var response= await _serviceServices.DeleteService(id);
            return response ? Ok() : NotFound();
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetServiceAll()
        {
            var services = await _serviceServices.GetServiceAll();
            return new OkObjectResult(services);
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateService([FromBody] Service appointment)
        {
          var response=  await _serviceServices.UpdateServices(appointment);
            return new OkObjectResult(response);
        }
        [HttpGet("Articulos")]
        // Endpoint auxiliar para obtener articulos relacionados desde la pantalla de servicios.
        public async Task<IActionResult> GetArticulos()
        {
            var articulos = await _serviceServices.GetArticulos();
            return new OkObjectResult(articulos);
        }

    }
}
