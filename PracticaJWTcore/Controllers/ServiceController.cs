﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Models;
using PracticaJWTcore.Services;
using System.Runtime.CompilerServices;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class ServiceController : Controller
    {
        private readonly IServiceServices _serviceServices;
        public ServiceController(IServiceServices servicesServices)
        {
            _serviceServices = servicesServices;
        }
        [HttpGet("{idServicioLong}")]
        public async Task<IActionResult> GetService(long idServicioLong)
        {
            var service = await _serviceServices.GetService(idServicioLong);
            return new OkObjectResult(service);
        }

        //En el post si o si debo enviarle igual el id del servicio = 0, no aceptan null
        [HttpPost]
        public async Task<IActionResult> CreateServices([FromBody] Service ServicesBody)
        {
            var service = await _serviceServices.CreateService(ServicesBody);
            return new CreatedResult($"http://localhost:7184/api/Service/{ServicesBody.ServiceId}",null);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(long id)
        {
            var response= await _serviceServices.DeleteService(id);
            return response ? Ok() : NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> GetServiceAll()
        {
            var services = await _serviceServices.GetServiceAll();
            return new OkObjectResult(services);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateService([FromBody] Service appointment)
        {
          var response=  await _serviceServices.UpdateServices(appointment);
            return new OkObjectResult(response);
        }
        [HttpGet("Articulos")]
        public async Task<IActionResult> GetArticulos()
        {
            var articulos = await _serviceServices.GetArticulos();
            return new OkObjectResult(articulos);
        }

    }
}
