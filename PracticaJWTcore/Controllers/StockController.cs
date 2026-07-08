using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Authorization;
using PracticaJWTcore.Dtos.Stock;
using PracticaJWTcore.Services;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("api/stock")]
    // Controller de stock: solo recibe la peticion HTTP y delega validaciones al service.
    public class StockController : Controller
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }
        [Authorize]
        [Permiso("Stock", "Leer")]
        [HttpGet("movimientos")]
        // GET /api/stock/movimientos lista movimientos sin exponer directamente la entidad EF.
        public async Task<IActionResult> GetMovimientos()
        {
            var movimientos = await _stockService.GetMovimientos();
            return Ok(movimientos);
        }
        [Authorize]
        [Permiso("Stock", "Leer")]
        [HttpGet("movimientos/{id}")]
        public async Task<IActionResult> GetMovimiento(long id)
        {
            var movimiento = await _stockService.GetMovimiento(id);
            if (movimiento == null)
                return NotFound();

            return Ok(movimiento);
        }
        [Authorize]
        [Permiso("Stock", "Crear")]
        [HttpPost("movimientos")]
        // POST registra un movimiento manual usando DTO; la regla de stock vive en StockService.
        public async Task<IActionResult> CreateMovimiento([FromBody] StockMovimientoRequestDto movimiento)
        {
            var result = await _stockService.CreateMovimiento(movimiento);
            if (!result.Success)
                return BadRequest(result.Message);

            return Created($"api/stock/movimientos/{result.Value!.IdStockMovimiento}", result.Value);
        }
        [Authorize]
        [Permiso("Stock", "Actualizar")]
        [HttpPut("movimientos/{id}")]
        // PUT valida la coherencia del id en el service y conserva el contrato de la ruta.
        public async Task<IActionResult> UpdateMovimiento(long id, [FromBody] StockMovimientoRequestDto movimiento)
        {
            var result = await _stockService.UpdateMovimiento(id, movimiento);
            if (!result.Success)
            {
                if (result.Code == "MOVEMENT_NOT_FOUND")
                    return NotFound();

                return BadRequest(result.Message);
            }

            return Ok(result.Value);
        }
        [Authorize]
        [Permiso("Stock", "Eliminar")]
        [HttpDelete("movimientos/{id}")]
        public async Task<IActionResult> DeleteMovimiento(long id)
        {
            var result = await _stockService.DeleteMovimiento(id);
            if (!result.Success)
                return NotFound();

            return NoContent();
        }
    }
}
