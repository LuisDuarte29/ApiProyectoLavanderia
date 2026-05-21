using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Dtos.Ventas;
using PracticaJWTcore.Services;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Controller de ventas: expone HTTP y deja reglas de stock, totales y persistencia en el service.
    public class VentasController : Controller
    {
        private readonly IVentasService _ventasService;

        public VentasController(IVentasService ventasService)
        {
            _ventasService = ventasService;
        }
        [Authorize]
        [HttpPost]
        // POST /api/Ventas crea una venta usando el contrato CreateVentaDto.
        public async Task<IActionResult> CreateVenta([FromBody] CreateVentaDto dto)
        {
            var result = await _ventasService.CreateVenta(dto);
            if (!result.Success)
                return BadRequest(result.Message);

            return Created($"api/ventas/{result.Value!.IdVenta}", result.Value);
        }
        [Authorize]
        [HttpGet]
        // GET /api/Ventas devuelve ventas proyectadas por el service/repository.
        public async Task<IActionResult> GetVentas()
        {
            var ventas = await _ventasService.GetVentas();
            return Ok(ventas);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVenta(long id)
        {
            var venta = await _ventasService.GetVenta(id);
            if (venta == null)
                return NotFound();

            return Ok(venta);
        }
        [Authorize]
        [HttpPut("{id}")]
        // PUT mantiene la ruta historica y traduce errores de negocio a status HTTP.
        public async Task<IActionResult> UpdateVenta(long id, [FromBody] UpdateVentaDto dto)
        {
            var result = await _ventasService.UpdateVenta(id, dto);
            if (!result.Success)
            {
                if (result.Code == "VENTA_NOT_FOUND")
                    return NotFound();

                return BadRequest(result.Message);
            }

            return Ok(result.Value);
        }
        [Authorize]
        [HttpDelete("{id}")]
        // DELETE delega la anulacion al service para restaurar stock y registrar trazabilidad.
        public async Task<IActionResult> DeleteVenta(long id)
        {
            var result = await _ventasService.DeleteVenta(id);
            if (!result.Success)
                return NotFound();

            return NoContent();
        }
    }
}
