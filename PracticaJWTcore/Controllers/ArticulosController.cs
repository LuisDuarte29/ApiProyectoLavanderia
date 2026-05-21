using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Dtos.Articulos;
using PracticaJWTcore.Services;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Controller de articulos: usa DTOs para separar el contrato API de la entidad Articulos.
    public class ArticulosController : Controller
    {
        private readonly IArticulosService _articulosService;

        public ArticulosController(IArticulosService articulosService)
        {
            _articulosService = articulosService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var articulos = await _articulosService.GetAll();
            return Ok(articulos);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var articulo = await _articulosService.GetById(id);
            if (articulo == null)
                return NotFound();

            return Ok(articulo);
        }
        [Authorize]
        [HttpPost]
        // POST /api/Articulos delega validaciones de nombre, precio, stock y categoria al service.
        public async Task<IActionResult> Create([FromBody] ArticuloRequestDto articulo)
        {
            var result = await _articulosService.Create(articulo);
            if (!result.Success)
                return BadRequest(result.Message);

            return Created($"api/articulos/{result.Value!.IdArticulo}", result.Value);
        }
        [Authorize]
        [HttpPut("{id}")]
        // PUT mantiene la ruta publica y deja la validacion de id/categoria en la capa de negocio.
        public async Task<IActionResult> Update(int id, [FromBody] ArticuloRequestDto articulo)
        {
            var result = await _articulosService.Update(id, articulo);
            if (!result.Success)
            {
                if (result.Code == "ARTICLE_NOT_FOUND")
                    return NotFound();

                return BadRequest(result.Message);
            }

            return Ok(result.Value);
        }
        [Authorize]
        [HttpDelete("{id}")]
        // DELETE se apoya en el service para bloquear eliminaciones con ventas o movimientos asociados.
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _articulosService.Delete(id);
            if (!result.Success)
            {
                if (result.Code == "ARTICLE_NOT_FOUND")
                    return NotFound();

                return BadRequest(result.Message);
            }

            return NoContent();
        }
    }
}
