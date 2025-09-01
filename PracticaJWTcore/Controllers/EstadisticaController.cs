using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Services;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstadisticaController : Controller
    {
        public readonly IEstadisticaServices _estadisticaServices;

        public EstadisticaController(IEstadisticaServices estadisticaServices)
        {
            _estadisticaServices = estadisticaServices ?? throw new ArgumentNullException(nameof(estadisticaServices));
        }
        [HttpGet("TipoLavado")]
        public async Task<IActionResult> GetTipoLavado()
        {
            var response = await _estadisticaServices.GetTipoLavado();
            return response !=null ?StatusCode( StatusCodes.Status200OK, new { lista= response }) : StatusCode(StatusCodes.Status204NoContent, new { string.Empty } );
        }
    }
}
