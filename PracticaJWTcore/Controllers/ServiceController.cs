using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Models;
using PracticaJWTcore.Services;
using System.Runtime.CompilerServices;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceController : Controller
    {
        private readonly ServiceServices _serviceServices;
        public ServiceController(ServiceServices servicesServices)
        {
            _serviceServices = servicesServices;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetService(long id)
        {
            var service = await _serviceServices.GetService(id);
            return new OkObjectResult(service);
        }
        [HttpPost]
        public async Task<IActionResult> CreateServices([FromBody] Service ServicesBody)
        {
            var service = await _serviceServices.CreateService(ServicesBody);
            return new CreatedResult($"http://locahost:7184/api/Service/{ServicesBody.ServiceId}",null);
        }
    }
}
