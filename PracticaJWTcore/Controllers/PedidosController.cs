using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticaJWTcore.Services;
using System.Threading.Tasks;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PedidosController : Controller
    {
        private readonly IPedidosServices _pedidos;
        public PedidosController(IPedidosServices pedidos)
        {
            _pedidos = pedidos;
        }
        [HttpGet]
        public async Task<IActionResult> GetPedidos()
        {
            var result = await _pedidos.GetPedidos();
            return new OkObjectResult(result);
        }
    }
}
