using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Services;
using System.Threading.Tasks;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    // Controller de pedidos: expone un listado armado como DTO para la pantalla de pedidos.
    public class PedidosController : Controller
    {
        private readonly IPedidosServices _pedidos;
        public PedidosController(IPedidosServices pedidos)
        {
            _pedidos = pedidos;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPedidos()
        {
            var result = await _pedidos.GetPedidos();
            return new OkObjectResult(result);
        }
    }
}
