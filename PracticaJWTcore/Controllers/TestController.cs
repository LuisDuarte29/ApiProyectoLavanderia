using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PracticaJWTcore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TestController : Controller
    {
        [AllowAnonymous] // <- Permite el acceso sin autenticación
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Conexión exitosa desde API");
        }
    }
}
